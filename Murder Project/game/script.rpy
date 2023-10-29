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
    from lib.inworld_api import PromptSender, AISessionHandler
    from lib.player import PlayerModel
    from lib.inworld_connection import InworldAPIClient

# PRO GAMER TIPS #
# default => Variable que se guarda por sesiones.
# define => Variable constante.

define aiko = Character("Aiko")
define debug = Character("Debug")

# This needs to be kept across sessions.
default info = PlayerModel("Juan", 20, "Male", "Scientist")
default info2 = PlayerModel("Alberto", 30, "Male", "Virus Researcher")

# uses the default .env if not provided.
default client = InworldAPIClient()

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

    # show aiko happy at half_size

    aiko "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In commodo risus metus, vitae tristique nisi tincidunt sed."
    aiko "Sample text..."

    python:
        # create a new session.
        senku_session = AISessionHandler(info, client, "doctor_lucas")

        # create a prompt based on Senku's session.
        prompt = PromptSender(senku_session)

        # save this aux variable.
        res = ""

        while True:
            res = renpy.input("Y bien cual es tu consulta?")

            # efectivamente, un loop.
            if res == "change":
                renpy.say(aiko, "swapping to alberto")
                senku_session.set_player_model(info2)
                renpy.say(aiko, f"Is valid {senku_session.is_valid()}")
                continue

            if res == "stop":
                break

            prompt.talk(res)

            text, feeling = prompt.show_response()

            for phrase in text:
                renpy.say(aiko, phrase)

            renpy.say(aiko, "FIN PRUEBA IA.")
            #block of code to run

    aiko "como tan muchacho [res]"

    # Presenta las líneas del diálogo.
    # Finaliza el juego:

    return
