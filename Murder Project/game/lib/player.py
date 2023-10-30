# object for handling player data.
# this should be loaded each time the game starts.
from lib.patterns import Singleton

class PlayerModel(metaclass=Singleton):
    def __init__(self, name : str, age : float, gender : str, role : str = ""):
        self.name = name
        self.age = age
        self.gender = gender
        self.game_role = role
    
    def get_info(self):
        return {
            "givenName": self.name,
            "age": self.age,
            "gender": self.gender,
            "role": self.game_role
        }

    def __str__(self):
        return f"[Name : {self.name}, Age: {self.age}, Gender: {self.gender}, Role: {self.game_role}]"