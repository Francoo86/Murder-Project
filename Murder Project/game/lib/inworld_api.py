# MINIMAL API FOR CONNECTION.
from connection import initialize_connection
from utils import copydict

# API KEY.
class BasePlayer:
    def __init__(self, user_name : str, age : float = 20, gender : str = ""):
        # self.player = char_name
        self.player_data = {
            "name": CHARACTER,
            "user": {
                "endUserId": "12345",
                # El nombre del jugador.
                "givenName": user_name,
                "age": age,
                "gender": gender
            }
        }

    def get_player_data(self) -> dict: 
        return self.player_data
    
    def get_user_data(self) -> dict:
        return self.player_data["user"]
    
    def _set_player_attribute(self, key : str, value : any) -> None:
        user = self.get_user_data()
        user[key] = value

    # This is for setting up the required data to send.
    def set_name(self, char : str):
        self._set_player_attribute("givenName", char)

    def set_role(self, role : str):
        self._set_player_attribute("role", role)
    
    def set_age(self, age : int):
        self._set_player_attribute("age", age)

class Prompt:
    def __init__(self, character : BasePlayer):
        # inject player to this.
        self.player = character
        self.session_handler = SessionHandler(character)
        self.previous_text = ""
        self.last_message = []
        self.last_prompt_data = {}

    def get_formatted_message(self) -> str:
        return " ".join(self.last_message)
    
    def get_user_last_text(self):
        return self.previous_text

    def send_text(self, text : str):
        session_id = self.session_handler.get_session_id()
        character_id = self.session_handler.get_player_session_id()

        if session_id is None:
            raise Exception("Character doesn't have a valid session ID.")
            # return FALLBACK_MESSAGE
        
        url = f'https://studio.inworld.ai/v1/{WORKSPACE_PATH}/sessions/{session_id}/sessionCharacters/{character_id}:sendText'
        
        copy_headers = HEADERS.copy()
        copy_headers["Grpc-Metadata-session-id"] = session_id

         # print("CURRENT JSON: ", copy_headers)

        self.previous_text = text
        
        conn = initialize_connection(url, "post", 0, json={"text": text}, headers=copy_headers)

        if not conn:
            return FALLBACK_MESSAGE

        json = conn.json()

        # for element in json:
            # print(element, json[element])

class SessionHandler:
    def __init__(self, player : BasePlayer):
        self.player = player
        self.session_id = None
        self.player_session_id = None

        # ensure this data will be used.
        self._old_player_data = copydict(self.player.get_player_data())

    def should_request_new_session(self) -> bool:
        old_data = self._old_player_data
        current_data = self.player.get_player_data()

        # not the same data so we copy it again.
        if old_data != current_data:
            self._old_player_data = copydict(current_data)
            return True
        
        # this is so nice, lets gooo
        return False
    def get_player_session_id(self) -> str:
        return self.player_session_id

    def get_session_id(self) -> str:
        # get it instantly.
        if not self.session_id or self.should_request_new_session():
            print("Requesting a new session...")
            self.request_new_session()

        return self.session_id

    def request_new_session(self) -> str:
        id = CHARACTER
        url = f'https://studio.inworld.ai/v1/{id}:openSession'
    
        conn = initialize_connection(url, "post", 0, json=self.player.get_player_data(), headers=HEADERS)

        if not conn:
            return None
        
        json = conn.json()
        session_id = json["name"]

        self.session_id = session_id
        self.player_session_id = json["sessionCharacters"][0]["character"]

        print("Requested data: ", self.session_id, self.player_session_id)

        return session_id
    
char = BasePlayer("Juanito", 20, "male")
char.set_role("chef")

prpt = Prompt(char)
prpt.send_text("How are you?")