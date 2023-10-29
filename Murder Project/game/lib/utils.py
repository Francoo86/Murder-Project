# most arcaic way to print this things but is for debugging purposes.
from copy import deepcopy

def copydict(tocopy : dict) -> dict:
    return deepcopy(tocopy)