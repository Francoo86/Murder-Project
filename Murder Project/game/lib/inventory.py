from items import BaseItem
from typing import List

class Inventory:
    def __init__(self):
        self.saved_items : list[BaseItem] = []
        
    def add_item(self):
        pass
    
    def remove_item(self):
        pass
    
    def get_items(self):
        return self.saved_items
    
    # basic search thing.
    def search_item(self, name : str):
        # nested ahh thing again.
        for item in self.saved_items:
            if item.name == name:
                return item
            
        return None
    
    
    