# Coloca el código de tu juego en este archivo.

# Declara los personajes usados en el juego como en el ejemplo:

# asignamos una imagen para la escena.
image bg santiasco = "santiasco.jpg"

# credits to noraneko games.
# only for testing btw.
image aiko normal = "aiko/Aiko_Halloween_Frown.png"
image aiko happy = "aiko/Aiko_Halloween_Smile.png"

# preparar fetching.
init python:
    from lib.connection import create_conn


define aiko = Character("Aiko")
define luigi = Character("Luigi")

transform half_size:
    zoom 0.4

# El juego comienza aquí.

label start:
    show text "Iniciando..."
    pause 1

    # Muestra una imagen de fondo: Aquí se usa un marcador de posición por
    # defecto. Es posible añadir un archivo en el directorio 'images' con el
    # nombre "bg room.png" or "bg room.jpg" para que se muestre aquí.

    scene bg santiasco

    # Muestra un personaje: Se usa un marcador de posición. Es posible
    # reemplazarlo añadiendo un archivo llamado "eileen happy.png" al directorio
    # 'images'.

    show aiko normal at half_size
    # show eileen happy
    aiko "Hola mundo!"
    aiko "Y para pensar señores."

    show aiko happy at half_size

    aiko "Imagina aprender las topologias en Cisco."
    aiko "Facil para el que estudio..."

    # Presenta las líneas del diálogo.
    #

    #mario "Hola soy Mario!!!"
    #luigi "Yo luigi!"

    # Finaliza el juego:

    return
