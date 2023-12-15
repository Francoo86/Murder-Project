import os
import shutil

REAL_PATH = os.path.dirname(os.path.realpath(__file__))
CURRENT_PATH = os.path.join(REAL_PATH, "PJ Expresiones")
MURDER_PROJECT_PATH = os.path.abspath(os.path.join(REAL_PATH, os.pardir))
TARGET_PATH = os.path.join(MURDER_PROJECT_PATH, "UnityVN-URP\Assets\Main\Resources\Characters")

# copy code i don't have time for this.
CHARACTERS = ["Ana", "Anastasia", "Jacinto", "Juan", "Lucia", "Luis", "Marcelo", "Patricia", "Pedro", "Santiago"]

for root, dirs, files in os.walk(CURRENT_PATH):
    for file in files:
        if not file.endswith("png"): continue
        
        full_path = os.path.join(root, file)
        new_name = file.replace(" ", "_").lower()
        new_path = os.path.join(root, new_name)

        os.rename(full_path, new_path)
        
for root, dirs, files in os.walk(CURRENT_PATH):
    character_name = root.split("\\")
    current_character = character_name[-1].strip()
    
    if not current_character in CHARACTERS: continue
    
    for file in files:
        if not file.endswith("png"): continue
        
        image_target_path = os.path.join(TARGET_PATH, current_character, "Images")
        target_path = os.path.join(root, file)
        
        shutil.copy2(target_path, image_target_path)