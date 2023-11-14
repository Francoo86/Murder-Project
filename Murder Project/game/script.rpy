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
    from lib.inworld_api import PromptSender

# PRO GAMER TIPS #
# default => Variable que se guarda por sesiones.
# define => Variable constante.
define debug = Character("Debug")

# this is the most important object here.
default inv = SingleInventory()
default prompt = PromptSender()

transform half_size:
    zoom 0.4
# El juego comienza aquí.

# TODO: Add pagination to decisions.
label game_prompt:
    $ narrator("Which character should i talk?", interact=False)
    $ new_char = renpy.display_menu(saved_tuples)
    $ sess = get_session_by_name(new_char)
    $ ai_dynamic = new_char
    $ prompt.set_session(sess)

    return

label start:        
    show text "Iniciando..."
    pause 1

    define knife = PassiveItem("Knife", "Knife used by the killer.")
    define rubber_duck = PassiveItem("Rubber Duck", "Why is there a rubber duck on my briefcase?!")
    define hammer = PassiveItem("Hammer", "To build things.")

    $ inv.add_item(knife)
    $ inv.add_item(rubber_duck)
    $ inv.add_item(hammer)
    # Muestra una imagen de fondo: Aquí se usa un marcador de posición por
    # defecto. Es posible añadir un archivo en el directorio 'images' con el
    # nombre "bg room.png" or "bg room.jpg" para que se muestre aquí.

    scene bg santiasco
    
    show screen inv_hud
    call game_prompt


    python:
        while True:
            res = renpy.input("Then, what are you asking for?")

            if res == "stop":
                break

            if res == "change":
                renpy.call("game_prompt", from_current=True)
                continue

            prompt.talk(res)

            text, feeling = prompt.show_response()
    
            for phrase in text:
                renpy.say(ai_dynamic, phrase)

    # Presenta las líneas del diálogo.
    # Finaliza el juego:

    return
