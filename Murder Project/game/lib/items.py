class BaseItem:
    def __init__(self) -> None:
        self.name = ""
        self.description = ""

    # Non abstract methods.
    def get_name(self):
        return self.name

    def get_description(self):
        return
    
    def set_name(self, name):
        self.name = name

class PassiveItem(BaseItem):
    def __init__(self):
        
        pass

    def use(self):
        pass

# bloc de notas?????
class TextItem(BaseItem):
    def __init__(self):
        self.current_text = ""
        pass

    def use(self):
        if not self.is_usable():
            return

    def is_usable(self):
        pass

