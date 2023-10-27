init python:
    from lib.patterns import Singleton

    class SinglePlayer(metaclass=Singleton):
        def __init__(self, info : PlayerInfo):
            self.info = info
            # HIGH COUPLING?!?!?!?!?
            
        def get_inventory(self):
            return self.inventory
        
        def use_item(self):
            pass

        def set_player_info(self, info : PlayerInfo):
            self.info = info

        def __str__(self):
            return f"Player Object {self.info}"