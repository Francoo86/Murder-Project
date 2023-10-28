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
HEADERS = {"Content-Type": "application/json", "authorization": KEY}

# key for saving the session.
GRPC_METADATA = "Grpc-Metadata-session-id"

# we need to reinvent the wheel rip.
class OpenAPIClient:
    # key should be defined.
    session = Session()
    rest_url = os.getenv("API_URL")
    headers = copydict(HEADERS)
    
        # we require the environment things.
    def __init__(self, key : str = KEY, workspace : str = WORKSPACE_PATH) -> None:
        self.key = key
        self.workspace = workspace

    def __connect_to_endpoint(self, endpoint : str, delay : int = 0, method : str = "post", data : dict = None, **kwargs):
        full_path = f"{self.rest_url}{WORKSPACE_PATH}{endpoint}"
        
        # connect to endpoint and thats it.
        connection = initialize_connection(self.session, full_path, 
                method, delay, json=data, headers=self.headers, **kwargs)

        if not connection:
            raise Exception(f"Can't connect to {full_path}")
        
        return connection.json()
    
    def request_character_session(self, character : str, ply_data : dict):
        session_endpoint = f'/characters/{character}:openSession'
        return self.__connect_to_endpoint(session_endpoint, data=ply_data)
    
    def send_prompt(self, sess_id : int, player_id : int, text : str):
        prompt_endpoint = f'/sessions/{sess_id}/sessionCharacters/{player_id}:sendText'
        
        # HACK: Add key then remove as this func requires this.
        self.headers[GRPC_METADATA] = sess_id
        json = self.__connect_to_endpoint(prompt_endpoint, data={"text": text})
        self.headers.pop(GRPC_METADATA)
        
        return json