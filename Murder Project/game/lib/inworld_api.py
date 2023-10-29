# if you want to test the module, you should remove the "lib." thing in this module.
from lib.utils import copydict
from lib.inworld_connection import OpenAPIClient
from lib.player import PlayerInfo

api_client = OpenAPIClient()

PLAYER_KEY = "user"

# NOT MVC
class SessionHandler:
    def __init__(self, player_info : PlayerInfo, client: OpenAPIClient, char : str) -> None:
        self.player_info : PlayerInfo = player_info
        self.character : str = char
        self.session_id : str = None
        self.player_session_id : str = None        

        # changed name to avoid weird things.
        self._old_player_session_data : dict = player_info.get_info()
        self.client = client
        
        self.request_new_session()

    def set_character(self, char : str):
        self.character = char

        # we are changing characters, the session should be changed.
        self.request_new_session()
        
    def set_client(self, client: OpenAPIClient):
        self.client = client
        
        self.request_new_session()

    def get_character(self):
        return self.character

    def get_session_data(self) -> tuple:
        id = self.get_session_id()
        player_id = self.get_player_session_id()
        
        return id, player_id

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
        data = self.client.request_character_session(self.character, {PLAYER_KEY: self._old_player_session_data})
        
        session_id = data["name"]
        player_id = data["sessionCharacters"][0]["character"]

        print("Requested data: ", session_id, player_id)
        
        self.session_id = session_id
        self.player_session_id = player_id
        
        return session_id, player_id

# VIEW
class CharacterResponse:
    def __init__(self):
        self.current_interaction = None
    
    def set_last_interaction(self, data : dict):
        self.current_interaction = data
        
    def get_feeling_data(self):
        data = self.get_response_feeling()
        return [data["behavior"], data["strength"]]
  
    def get_response_text(self):
        return self.current_interaction["textList"]
    
    def get_response_feeling(self):
        return self.current_interaction["emotion"]
    
    def get_response(self):
        return self.get_response_text(), self.get_feeling_data()

# CONTROLLER.    
class PromptSender:
    def __init__(self, sess : SessionHandler) -> None:
        self.current_session = sess
        self.view = CharacterResponse()
        
    def __do_action(self, **kwargs):
        sess = self.current_session
        interaction = sess.client.send_prompt(sess.session_id, sess.player_session_id, **kwargs)
        # save data to the interactor.
        self.view.set_last_interaction(interaction)
        
    def talk(self, text : str):
        self.__do_action(text=text)
    
    def update_environment(self, trigger : str, params : dict = None):
        self.__do_action(trigger_name=trigger, scene_params=params)

    def show_response(self):
        return self.view.get_response()