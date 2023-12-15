import os
import time

# wrote this because im lazy for creating folders for each character.
CURRENT_PATH = os.path.dirname(os.path.realpath(__file__))

CHARACTERS = ["Ana", "Anastasia", "Jacinto", "Juan", "Lucia", "Luis", "Marcelo", "Patricia", "Pedro", "Santiago"]

for character in CHARACTERS:
    joined = os.path.join(CURRENT_PATH, character)
    images_path = os.path.join(joined, "Images")
    
    os.makedirs(images_path)