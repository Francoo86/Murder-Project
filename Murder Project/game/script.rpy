# Coloca el código de tu juego en este archivo.

# Declara los personajes usados en el juego como en el ejemplo:

define mario = Character("Mario")
define luigi = Character("Luigi")

# El juego comienza aquí.

label start:
    python:
        print("test!!!!")

    # Muestra una imagen de fondo: Aquí se usa un marcador de posición por
    # defecto. Es posible añadir un archivo en el directorio 'images' con el
    # nombre "bg room.png" or "bg room.jpg" para que se muestre aquí.

    scene bg room

    # Muestra un personaje: Se usa un marcador de posición. Es posible
    # reemplazarlo añadiendo un archivo llamado "eileen happy.png" al directorio
    # 'images'.

    # show eileen happy

    # Presenta las líneas del diálogo.

    mario "Hola soy Mario!!!"

    luigi "Yo luigi!"

    # Finaliza el juego:

    return
