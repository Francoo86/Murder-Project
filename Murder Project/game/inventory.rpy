init python:
    from lib.patterns import Singleton

    # MOST IMPORTANT ITEM THING.
    class BaseItem:
        def __init__(self) -> None:
            self.name = ""
            self.description = ""
            self.image_path = ""

        # Non abstract methods.
        def set_description(self, desc : str):
            self.description = desc
        
        def set_name(self, name : str):
            self.name = name
            
        def set_image(self, img_path : str):
            self.image_path = img_path

    # INHERITED ITEMS.
    class PassiveItem(BaseItem):
        pass

    class ActiveItem(BaseItem):
        def __init__(self) -> None:
            super().__init__()

        def use(self):
            pass

    # HERE WE SAVE THE PLAYER DATA.
    # only one inventory.
    class SingleInventory(metaclass=Singleton):
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