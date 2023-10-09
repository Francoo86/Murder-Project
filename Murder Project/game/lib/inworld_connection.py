from abc import ABC, abstractmethod
from lib.connection import initialize_connection
from lib.utils import copydict

# LA LLAVE DE API.
KEY = "Basic " + "LLAVE"
# EL PATH DEL ESPACIO DE TRABAJO.
WORKSPACE_PATH = "workspaces/{ID}"
# EL PERSONAJE COMO TAL.
CHARACTER = WORKSPACE_PATH + "/characters/madame_fortune_teller"
# Headers, dejarlos así nomás.
HEADERS = {"Content-Type": "application/json", "authorization": KEY}

# key for saving the session.
GRPC_METADATA = "Grpc-Metadata-session-id"

# Yeah, the metamorphosis thing is real.
class AbstractConnection(ABC):
    # in the case we use get we change it.
    def __init__(self, data : dict):
        self.data = data
        self.method = "post"
        self.delay = 0
        self.is_setup = False
        self.headers = HEADERS

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
        connection = initialize_connection(url, self.method, self.delay, json=self.data, headers=self.headers, **kwargs)

        if not connection:
            raise Exception(fallback_msg)
        
        return connection.json()

class OpenSessionAPIConnection(AbstractConnection):
    def __init__(self, data : dict):
        super().__init__(data)

    def setup_data(self):
        name_json = {"name": CHARACTER}
        name_json.update(self.data)

        # save the user data here.
        name_json["user"] = self.data

        # this guy is the merged.
        self.data = name_json
    
    def connect(self) -> tuple:
        # data to do the connection.
        url = f'https://studio.inworld.ai/v1/{CHARACTER}:openSession'
        json = self.connect_url(url, fallback_msg="Can't connect to OpenSession API.")

        # get the connection important things.
        session_id = json["name"]
        player_id = json["sessionCharacters"][0]["character"]

        print("Requested data: ", session_id, player_id)

        return session_id, player_id

class SendTextAPIConnection(AbstractConnection):
    def setup_data(self):
        self.session_id = self.data["ID"]
        self.player_id = self.data["PlayerID"]
        self.text_send = self.data["Text"]

        # do this in this case because is the json data.
        self.data = {"text": self.text_send}
        
    def __init__(self, data : dict):
        # we init this.
        super().__init__(data)
        # self.setup_data()

        new_headers = copydict(HEADERS)
        new_headers[GRPC_METADATA] = self.session_id

        self.set_headers(new_headers)
        
    def connect(self) -> dict:
        url = f'https://studio.inworld.ai/v1/{WORKSPACE_PATH}/sessions/{self.session_id}/sessionCharacters/{self.player_id}:sendText'
        json = self.connect_url(url, fallback_msg="Can't connect to SendText API.")
        return json

# TODO: Implement.
# I feel that this one is not very important.
class SendTriggerAPIConnection(AbstractConnection):
    pass