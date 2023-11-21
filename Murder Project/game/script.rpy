﻿# Coloca el código de tu juego en este archivo.

# Declara los personajes usados en el juego como en el ejemplo:

# asignamos una imagen para la escena.
image bg santiasco:
    zoom 2
    "granja.jpg"

image bg iglesia:
    zoom 2
    "iglesia.jpg"

# credits to noraneko games.
# only for testing btw.
image doctor_lucas normal = "doctor_lucas/doctor_lucas_Halloween_Frown.png"
image doctor_lucas happy = "doctor_lucas/doctor_lucas_Halloween_Smile.png"

image marcelo normal = "chars/viejo.png"
image luis normal = "chars/nose.png"

# preparar fetching.
init python:
    from lib.inworld_api import PromptSender

# PRO GAMER TIPS #
# default => Variable que se guarda por sesiones.
# define => Variable constante.
define debug = Character("Debug")
define luis = Character("Luis")
define marcelo = Character("Marcelo")

# this is the most important object here.
default inv = SingleInventory()
default prompt = PromptSender()

transform half_size:
    zoom 0.5
    ypos 0.75

transform luis_size:
    zoom 0.5
    ypos 0.75

# TODO: Add pagination to decisions.
label game_prompt:
    $ narrator("Which character should i talk?", interact=False)
    $ new_char = renpy.display_menu(saved_tuples)
    $ sess = get_session_by_name(new_char)
    $ ai_dynamic = new_char
    $ prompt.set_session(sess)
    $ renpy.show(f"{ai_dynamic.lower()} normal", [left, half_size])

    return

label game_prompt_demo:
    call game_prompt

    python:
        while True:
            res = renpy.input("Then, what are you asking for?")

            if res == "stop":
                break

            if res == "change":
                renpy.hide(f"{ai_dynamic.lower()} normal")
                renpy.call("game_prompt", from_current=True)
                continue

            prompt.talk(res)

            text, feeling = prompt.show_response()

            for phrase in text:
                renpy.say(ai_dynamic, phrase)

    return

label demonstration_intro:
    narrator "Nada mejor que un buen día soleado en estos tiempos, donde el pueblo se muestra tranquilo..."

    show marcelo normal at left, half_size with dissolve

    marcelo "Buenos días Lucho."

    show luis normal at right, half_size with dissolve

    luis "Today we speak only in english, so yeah, hi Marcelo."
    luis "How are you doing today?"

    marcelo "Thanks i am doing great right now, but there is something that gets me worried in these times."
    marcelo "You probably know about what i'm talking"
    
    luis "What?"
    luis "The murdering that happened like 3 days ago?"

    marcelo "Yeah."
    
    luis "Do you wanna come to the church? To talk about this topic without worrying the town."

    marcelo "Sure."

    hide luis normal with dissolve
    hide marcelo normal with dissolve

    show text "Going to the church..."

    scene bg iglesia with dissolve

    show luis normal at right, half_size with dissolve

    luis "Now here we are, so, whats the most important element about the murdering?"

    show marcelo normal at left, half_size with dissolve

    marcelo "Let our detective investigate that."

    narrator "'Yeah, i will search all places to get some evidence.'"

    marcelo "Thanks"
    luis "Thanks"

    narrator "No problem."

    hide luis normal with dissolve
    hide marcelo normal with dissolve

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
    
    menu dev_menu:
        "Do demo of game.":
            call demonstration_intro
        "Go to prompt demo.":
            call game_prompt_demo

    return
