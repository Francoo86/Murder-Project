# if you want to test the module, you should remove the "lib." thing in this module.
from lib.utils import copydict
from lib.inworld_connection import OpenAPIClient
from lib.player import PlayerInfo

api_client = OpenAPIClient()

class SessionHandler:
    def __init__(self, player_info : PlayerInfo, char : str) -> None:
        self.player_info : PlayerInfo = player_info
        self.character : str = char
        self.session_id : str = None
        self.player_session_id : str = None        

        # changed name to avoid weird things.
        self._old_player_session_data : dict = player_info.get_info()

    def set_character(self, char : str):
        self.character = char

        # we are changing characters, the session should be changed.
        self.request_new_session()

    def get_character(self):
        return self.character

    def get_session_data(self) -> tuple:
        id = self.get_session_id()
        player_id = self.get_player_session_id()
        
        return id, player_id
        # return {"ID" : id, "PlayerID": player_id, "Character" : self.character}

    def should_request_new_session(self) -> bool:
        old_data = self._old_player_session_data
        current_data = self.player_info.get_info()

        # not the same data so we copy it again.
        if old_data != current_data:
            self._old_player_session_data = current_data
            return True

        return False
    def get_player_session_id(self) -> str:
        return self.player_session_id

    def get_session_id(self) -> str:
        # get it instantly.
        if not self.session_id or self.should_request_new_session():
            print("Requesting a new session...")
            self.request_new_session()

        return self.session_id

    # player thing.
    def set_player_info(self, player_info : PlayerInfo) -> None:
        self.player_info = player_info

    def request_new_session(self) -> str:
        # send character and the user_data.
        data = api_client.request_character_session(self.character, {"user": self._old_player_session_data})
        
        session_id = data["name"]
        player_id = data["sessionCharacters"][0]["character"]

        print("Requested data: ", session_id, player_id)
        
        self.session_id = session_id
        self.player_session_id = player_id
        
        return session_id, player_id

class Prompt:
    def __init__(self, session : SessionHandler) -> None:
        # inject session to this.
        self.session_handler = session
        self.previous_text = ""

        # initialize it in none.
        self.last_message : list = None
        self.last_prompt_data : dict = None

    # we can have multiple sessions.
    def set_session_handler(self, session : SessionHandler) -> None:
        self.session_handler = session

    # no setters because there are no need to set things.
    def get_formatted_message(self) -> str:
        return " ".join(self.last_message)
    
    """
    def get_data_to_send(self) -> dict:
        data = self.session_handler.get_session_data()
        data.update({"Text" : self.previous_text})

        return data"""

    def send_text(self, text : str) -> list:
        self.previous_text = text
        id, ply_id = self.session_handler.get_session_data()
        data = api_client.send_prompt(id, ply_id, text)

        # save this important data.
        self.last_prompt_data = data

        # we want to get the response thing.
        self.last_message = data["textList"]

        return self.last_message