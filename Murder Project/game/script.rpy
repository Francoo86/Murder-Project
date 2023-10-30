# Coloca el código de tu juego en este archivo.

# Declara los personajes usados en el juego como en el ejemplo:

# asignamos una imagen para la escena.
image bg santiasco:
    zoom 2
    "granja.jpg"

# credits to noraneko games.
# only for testing btw.
image doctor_lucas normal = "doctor_lucas/doctor_lucas_Halloween_Frown.png"
image doctor_lucas happy = "doctor_lucas/doctor_lucas_Halloween_Smile.png"

# preparar fetching.
init python:
    from lib.inworld_api import PromptSender, AISessionHandler
    from lib.player import PlayerModel
    from lib.inworld_connection import InworldAPIClient

    # info = PlayerModel("Juan", 20, "Male", "Scientist")
    client = InworldAPIClient()

# PRO GAMER TIPS #
# default => Variable que se guarda por sesiones.
# define => Variable constante.
define ai_dynamic_base = Character("AIChar")
define debug = Character("Debug")

# This needs to be kept across sessions.

# uses the default .env if not provided.
default info = PlayerModel("Luisito", 35, "Male", "Detective")

# this is the most important object here.
default client = InworldAPIClient()
define prompt = PromptSender()

transform half_size:
    zoom 0.4

# selects from the AIs some alternatives.
label selection:
    $ selected_character = None
    $ sess_pedro = AISessionHandler(info, client, "viejo_pedro")
    $ sess_priest = AISessionHandler(info, client, "priest")
    $ sess_lucas = AISessionHandler(info, client, "doctor_lucas")

    menu:
        "Which AI should i talk to?"

        "Talk with Doctor Lucas":
            debug "Talking with Lucas"
            $ prompt.set_session(sess_lucas)
            $ ai_dynamic_base = "Doctor Lucas"
        "Talk with Pedro":
            debug  "Talking with Pedro"
            $ prompt.set_session(sess_pedro)
            $ ai_dynamic_base = "Pedro"
        "Talk with Priest":
            debug "Talking with Priest"
            $ prompt.set_session(sess_priest)
            $ ai_dynamic_base = "Priest"

    return

# El juego comienza aquí.

label start:        
    show text "Iniciando..."
    pause 1

    # Muestra una imagen de fondo: Aquí se usa un marcador de posición por
    # defecto. Es posible añadir un archivo en el directorio 'images' con el
    # nombre "bg room.png" or "bg room.jpg" para que se muestre aquí.

    scene bg santiasco
    call selection

    python:
        # create a new session.
        # typical loop for testing these kind of stuffs.
        while True:
            res = renpy.input("Then, what are you asking for?")

            if res == "stop":
                break

            if res == "change":
                renpy.call("selection", from_current=True)
                continue

            prompt.talk(res)

            text, feeling = prompt.show_response()

            for phrase in text:
                renpy.say(ai_dynamic_base, phrase)

    # Presenta las líneas del diálogo.
    # Finaliza el juego:

    return
