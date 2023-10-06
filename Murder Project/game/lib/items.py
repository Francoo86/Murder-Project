from abc import ABC, abstractmethod

# Abstract class that saves items.
class BaseItem(ABC):
    def __init__(self) -> None:
        self._name = ""
        self._description = ""

    # Should be implemented.
    @abstractmethod
    def use(self):
        pass

    @abstractmethod
    def shoud_use(self):
        pass
    
    # Non abstract methods.
    def get_name(self):
        return self._name
    
    def set_name(self, name):
        self._name = name