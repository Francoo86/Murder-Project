from abc import ABC, abstractmethod
from lib.connection import initialize_connection
from lib.utils import copydict
from requests import Session
from urllib.parse import urljoin

# thanks pip thing.
# this library doesn't exist in base renpy sdk.
from lib.dotenv import load_dotenv
import os

# renpy loads here.
# WORKAROUND: Import the path thing to fix the .env not found error.
SCRIPT_PATH = os.path.dirname(os.path.abspath(__file__))
GAME_PATH = os.path.dirname(os.path.dirname(SCRIPT_PATH))
load_dotenv(os.path.join(GAME_PATH, ".env"))

# this data is important for the game api.
KEY = os.getenv("API_KEY")
WORKSPACE_PATH = os.getenv("WORKSPACE_PATH")

# key for saving the session.
GRPC_METADATA = "Grpc-Metadata-session-id"

# TODO: Allow for 4 sessions.
class OpenAPIClient:
    session = Session()
    rest_url = os.getenv("API_URL")

    # we require the environment things.
    def __init__(self, key : str = KEY, workspace : str = WORKSPACE_PATH) -> None:
        # headers don't need to be static, as they be more prone to change.
        self.headers = {"Content-Type": "application/json"}
        self.set_auth_data(key, workspace)
    
    # para los panas
    def set_auth_data(self, key : str, workspace : str):
        self.key = key
        self.workspace = workspace
        self.headers["authorization"] = key

    def __call_api(self, endpoint : str, delay : int = 0, method : str = "post", data : dict = None, **kwargs):
        full_path = f"{self.rest_url}{self.workspace}{endpoint}"

        # connect to endpoint and thats it.
        connection = initialize_connection(self.session, full_path, 
                method, delay, json=data, headers=self.headers, **kwargs)

        if not connection:
            raise Exception(f"Can't connect to {full_path}")
        
        return connection.json()
    
    # this is mostly for sessions.
    def __call_with_grpc(self, endpoint : str, sess_id : str, data : dict):
        # HACK: Add key then remove as this func requires this.
        self.headers[GRPC_METADATA] = sess_id
        json = self.__call_api(endpoint, data=data)
        self.headers.pop(GRPC_METADATA)
        
        return json
    
    def request_character_session(self, character : str, ply_data : dict):
        session_endpoint = f'/characters/{character}:openSession'
        return self.__call_api(session_endpoint, data=ply_data)
    
    def send_prompt(self, sess_id : str, player_id : int, text : str):
        prompt_endpoint = f'/sessions/{sess_id}/sessionCharacters/{player_id}:sendText'
        return self.__call_with_grpc(prompt_endpoint, sess_id, {"text" : text})
    
    def send_goal(self, sess_id : str, player_id : str, trigger_name : str):
        goal_endpoint = f"/sessions/{sess_id}/sessionCharacters/{player_id}:sendTrigger"
        return self.__call_with_grpc(goal_endpoint, sess_id, 
                                    {"triggerEvent": { "trigger":f"{self.workspace}/triggers/{trigger_name}"}})