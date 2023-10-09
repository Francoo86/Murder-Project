from abc import ABC, abstractmethod
from lib.connection import initialize_connection
from lib.utils import copydict

# Headers, dejarlos así nomás.
HEADERS = {"Content-Type": "application/json", "authorization": KEY}

# Yeah, the metamorphosis thing is real.
class AbstractConnection(ABC):
    # in the case we use get we change it.
    def __init__(self, data : any):
        self.data = data
        self.method = "post"
        self.delay = 0

    @abstractmethod
    def connect(self, delay : int = 0, headers = HEADERS) -> any:
        pass

    def setup_data(self):
        pass

    def set_method(self, method : str):
        self.method = method

    def set_delay(self, delay : int):
        self.delay = delay

class OpenSessionAPIConnection(AbstractConnection):
    def __init__(self, data : dict):
        self.set_player_data(data)

    def set_method(self, method: str):
        return super().set_method(method)

    # only saves the reference though.
    def set_player_data(self, new_data : dict):
        # basically we need to merge these guys.
        # the character is not relevant to player class.
        name_json = {"name": CHARACTER}
        name_json.update(new_data)

        # save the user data here.
        name_json["user"] = new_data

        # this guy is the merged.
        self.player_data = name_json
    
    def connect(self, delay: int = 0, headers = HEADERS) -> tuple:
        # data to do the connection.
        id = CHARACTER
        url = f'https://studio.inworld.ai/v1/{id}:openSession'

        connection = initialize_connection(url, "post", delay=delay, json=self.player_data, headers=headers)

        if not connection:
            raise Exception("Can't connect to open session API.")
        
        json = connection.json()

        # get the connection important things.
        session_id = json["name"]
        player_id = json["sessionCharacters"][0]["character"]

        print("Requested data: ", session_id, player_id)

        return session_id, player_id

class SendTextAPIConnection(AbstractConnection):
    def __init__(self, session_id : str, player_id : str, text : str):
        self.session_id = session_id
        self.player_id = player_id
        self.text_send = text

        # init headers here.
        self.custom_headers = copydict(HEADERS)
        self.custom_headers["Grpc-Metadata-session-id"] = session_id
        
    def connect(self, delay: int = 0, headers=HEADERS) -> dict:
        url = f'https://studio.inworld.ai/v1/{WORKSPACE_PATH}/sessions/{self.session_id}/sessionCharacters/{self.player_id}:sendText'
        connection = initialize_connection(url, "post", delay=delay, json={"text": self.text_send}, headers=self.custom_headers)
        
        if not connection:
            raise Exception("Can't connect to the send text API.")
        
        return connection.json()

# TODO: Implement.
class SendTriggerAPIConnection(AbstractConnection):
    pass