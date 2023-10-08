from abc import ABC, abstractmethod

# Abstract class that saves items.
class BaseItem(ABC):
    def __init__(self) -> None:
        self.name = ""
        self.description = ""

    # Should be implemented.
    @abstractmethod
    def use(self):
        pass

    @abstractmethod
    def is_usable(self):
        pass
    
    # Non abstract methods.
    def get_name(self):
        return self.name
    
    def set_name(self, name):
        self.name = name

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

