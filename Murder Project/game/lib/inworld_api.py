# if you want to test the module, you should remove the "lib." thing in this module.
from lib.inworld_connection import InworldAPIClient
from lib.player import PlayerModel
from lib.patterns import Singleton

PLAYER_KEY = "user"

# NOT MVC
class AISessionHandler:
    def __init__(self, player_model : PlayerModel, client: InworldAPIClient, char : str = "") -> None:
        self.character : str = char
        self.session_id : str = None
        self.player_session_id : str = None        
        self.client = client
        
        # player model handling.
        self.player_model = player_model
        self.current_data = player_model.get_info()
        
    def set_character(self, char : str):
        self.character = char

        # we are changing characters, the session should be changed.
        self.request_new_session()
        
    def set_player_model(self, ply_info : PlayerModel):
        self.player_model = ply_info
        
    def set_client(self, client: InworldAPIClient):
        self.client = client
        
        self.request_new_session()
        
    def is_valid(self):
        return (self.session_id is not None) and self.is_reliable()

    def is_reliable(self) -> bool:
        old_data = self.current_data
        current_data = self.player_model.get_info()
        return old_data == current_data

    def prepare(self) -> bool:
        # get it instantly.
        if not self.is_valid():
            self.current_data = self.player_model.get_info()
            print("Requesting a new session...")
            self.request_new_session()

        return True

    def request_new_session(self) -> None:
        # send character and the user_data.
        data = self.client.request_character_session(self.character, {PLAYER_KEY: self.current_data})

        if data is None:
            return
        
        session_id = data["name"]
        player_id = data["sessionCharacters"][0]["character"]

        print("Requested data: ", session_id, player_id)
        
        self.session_id = session_id
        self.player_session_id = player_id

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
        if self.current_interaction is None:
            return ["Sorry i'm sleeping right now, maybe try by resetting the game, or report to the developers..."], ["SLEEPY"]

        return self.get_response_text(), self.get_feeling_data()

# CONTROLLER.    
class PromptSender(metaclass=Singleton):
    def __init__(self, sess : AISessionHandler = None) -> None:
        self.current_session = sess
        self.view = CharacterResponse()
        
    def set_session(self, sess : AISessionHandler):
        self.current_session = sess
        
    def __do_request(self, **kwargs):
        if self.current_session is None:
            return

        sess = self.current_session
        # will initialize once.
        sess.prepare()
        
        interaction = sess.client.send_prompt(sess.session_id, sess.player_session_id, **kwargs)
        # save data to the interactor.
        self.view.set_last_interaction(interaction)
        
    def talk(self, text : str):
        self.__do_request(text=text)
    
    def update_environment(self, trigger : str, params : dict = None):
        self.__do_request(trigger_name=trigger, scene_params=params)

    def show_response(self):
        return self.view.get_response()