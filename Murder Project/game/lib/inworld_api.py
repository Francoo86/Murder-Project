# MINIMAL API FOR CONNECTION.
from connection import initialize_connection

# Información del Inworld.
KEY = "Basic " + "LA LLAVE"
CHARACTER = "del workspace"
HEADERS = {"Content-Type": "application/json", "authorization": KEY}
URL = f'https://studio.inworld.ai/v1/{CHARACTER}:simpleSendText'
FALLBACK_MESSAGE = "At this moment i can't process your request, try again later!"

# API KEY.
class CharacterHandler:
    def __init__(self, char_name : str):
        self.character = char_name
        self.session_id = None
        self.current_data = None
        self.input_text = ""

        self.character_data = {
            "character": CHARACTER,
            "text": "",
            "endUserFullName": char_name,
            "endUserId": "12345"
        }

    def set_text(self, text : str):
        # Prepare this thingy for json.
        self.input_text = text
        self.character_data["text"] = text
    
    def send_text(self, text : str):
        self.set_text(text)
        conn = initialize_connection(URL, "post", 0, json=self.character_data, headers=HEADERS)

        if not conn:
            print("We can't connect to the site. Code: ", conn.status_code)
            return FALLBACK_MESSAGE
        
        json_data = conn.json()

        # Save this data for previous conversation
        self.current_data = json_data

        # This is a list thing.
        return self.current_data["textList"]