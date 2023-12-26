# contar archivos de cs.

import os

FILE_PATH = os.path.dirname(os.path.realpath(__file__))
count = 0

for root, dir, files in os.walk(FILE_PATH):
    for file in files:
        if file.endswith(".cs"):
            count = count + 1
            
            
    # print(root, dir, files)
print(count)