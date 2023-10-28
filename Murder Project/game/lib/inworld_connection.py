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

# SHARES THE SAME WORKSPACE PATH.
FULL_SERVICE = urljoin(os.getenv("API_URL"), WORKSPACE_PATH)


# we need to reinvent the wheel rip.
class APIClient:
    def __init__(self) -> None:
        self.key : str
        self.path : str

class AbstractConnection(ABC):
    # in the case we use get we change it.
    # static session.
    # we are connecting to the same service, the only thing that changes is the endpoint.
    session = Session()
    
    def __init__(self, data : dict = None):
        self.method = "post"
        self.delay = 0
        self.headers = HEADERS
        # we avoid a bit of overhead.
        # self.session = Session()
        self.data = data
        self.setup_data()

    @abstractmethod
    def connect(self) -> any:
        pass
    
    @abstractmethod
    def setup_data(self) -> None:
        pass

    def set_method(self, method : str) -> None:
        self.method = method

    def set_delay(self, delay : int) -> None:
        self.delay = delay

    def set_data(self, data : dict) -> None:
        self.data = data

        # new data provided so we setup it.
        self.setup_data()

    def set_headers(self, data : dict) -> None:
        self.headers = data

    def connect_url(self, url : str, fallback_msg : str, **kwargs) -> dict:
        connection = initialize_connection(self.session, url, self.method, self.delay, json=self.data, headers=self.headers, **kwargs)

        if not connection:
            raise Exception(fallback_msg)
        
        return connection.json()

class OpenSessionAPIConnection(AbstractConnection):
    def __init__(self, data : dict):
        super().__init__(data)

    def setup_data(self):
        self.character = self.data["character"]

        # Remove this data as we don't want to send it in the POST parameters.
        self.data.pop("character")
    
    def connect(self) -> tuple:
        # data to do the connection.
        url = f'/characters/{self.character}:openSession'
        json = self.connect_url(url, fallback_msg="Can't connect to OpenSession API.")

        # get the connection important things.
        session_id = json["name"]
        player_id = json["sessionCharacters"][0]["character"]

        print("Requested data: ", session_id, player_id)

        return session_id, player_id

class PromptAPIConnection(AbstractConnection):
    def setup_data(self):
        self.session_id = self.data["ID"]
        self.player_id = self.data["PlayerID"]
        self.text_send = self.data["Text"]

        # do this in this case because is the json data.
        # HACK: Reassign the data as we need to send the json stuff.
        self.data = {"text": self.text_send}
        
    def __init__(self, data : dict):
        # we init this.
        super().__init__(data)
        # self.setup_data()

        new_headers = copydict(HEADERS)
        new_headers[GRPC_METADATA] = self.session_id

        self.set_headers(new_headers)
        
    def connect(self) -> dict:
        url = f'/sessions/{self.session_id}/sessionCharacters/{self.player_id}:sendText'
        json = self.connect_url(url, fallback_msg="Can't connect to SendText API.")
        return json

# TODO: Implement.
# I feel that this one is not very important.
class SendTriggerAPIConnection(AbstractConnection):
    pass