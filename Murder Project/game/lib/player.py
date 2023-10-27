from lib.patterns import Singleton
from lib.inventory import Inventory

# object for handling player data.
# this should be loaded each time the game starts.
class PlayerInfo:
    def __init__(self, name : str, age : float, gender : str, role : str = ""):
        self.name = name
        self.age = age
        self.gender = gender
        self.game_role = role
    
    def get_info_as_dict(self):
        return {
            "givenName": self.name,
            "age": self.age,
            "gender": self.gender,
            "role": self.game_role
        }

    def set_game_role(self, role : str):
        self.game_role = role
        
    def __str__(self):
        return f"[Name : {self.name}, Age: {self.age}, Gender: {self.gender}, Role: {self.game_role}]"

class SinglePlayer(metaclass=Singleton):
    def __init__(self, info : PlayerInfo):
        self.info = info
        self.inventory = Inventory()
        
    def get_inventory(self):
        return self.inventory
    
    def use_item(self):
        pass

    def set_player_info(self, info : PlayerInfo):
        self.info = info

    def __str__(self):
        return f"Player Object {self.info}"