# Coloca el código de tu juego en este archivo.

# Declara los personajes usados en el juego como en el ejemplo:

# asignamos una imagen para la escena.
image bg santiasco:
    zoom 2
    "granja.jpg"

# credits to noraneko games.
# only for testing btw.
image aiko normal = "aiko/Aiko_Halloween_Frown.png"
image aiko happy = "aiko/Aiko_Halloween_Smile.png"

# preparar fetching.
init python:
    from lib.inworld_api import Prompt, SessionHandler
    from lib.player import PlayerInfo
    from lib.inworld_connection import OpenAPIClient

# PRO GAMER TIPS #
# default => Variable que se guarda por sesiones.
# define => Variable constante.

define aiko = Character("Aiko")
define debug = Character("Debug")

# This needs to be kept across sessions.
default info = PlayerInfo("Juan", 20, "Male", "Scientist")

# uses the default .env if not provided.
default client = OpenAPIClient()

# load player data.
# default ply = SinglePlayer(info)

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

    menu:
        "Hablar con Pedro.":
            debug "hablando con Pedro"
        "Hablar con Juan":
            debug  "Hablando con Juan."
        "Hablar con Diego":
            debug "Hablando con Diego"

    # Muestra un personaje: Se usa un marcador de posición. Es posible
    # reemplazarlo añadiendo un archivo llamado "eileen happy.png" al directorio
    # 'images'.

    # show aiko normal at half_size
    # show eileen happy
    aiko "Hola mundo!"
    aiko "Y para pensar señores."

    # show aiko happy at half_size

    aiko "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In commodo risus metus, vitae tristique nisi tincidunt sed."
    aiko "Sample text..."

    python:
        # create a new session.
        senku_session = SessionHandler(info, client, "doctor_lucas")

        # create a prompt based on Senku's session.
        prpt = Prompt(senku_session)

        # save this aux variable.
        res = ""

        while True:
            res = renpy.input("Y bien cual es tu consulta?")

            # efectivamente, un loop.
            if res == "stop":
                break

            text = prpt.send_text(res)

            for phrase in text:
                renpy.say(aiko, phrase)

            renpy.say(aiko, "FIN PRUEBA IA.")
            #block of code to run

    aiko "como tan muchacho [res]"

    # Presenta las líneas del diálogo.
    # Finaliza el juego:

    return
