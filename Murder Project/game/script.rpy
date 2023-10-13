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
    from lib.inworld_api import Player, Prompt, SessionHandler


define aiko = Character("Aiko")

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

    # show aiko normal at half_size
    # show eileen happy
    aiko "Hola mundo!"
    aiko "Y para pensar señores."

    # show aiko happy at half_size

    aiko "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In commodo risus metus, vitae tristique nisi tincidunt sed. Nunc ut neque vel purus hendrerit egestas et nec erat. Donec vitae elit a nisi fermentum rhoncus nec condimentum magna. Pellentesque eleifend sagittis dui, et tincidunt nulla mattis ac. Duis pellentesque ante sed dui consectetur pellentesque. Aenean scelerisque id dui a tristique. Curabitur efficitur imperdiet arcu,
    in vulputate eros commodo nec. Praesent elementum dui faucibus ligula accumsan faucibus. Cras in scelerisque est."
    aiko "Sample text..."

    python:
        player = Player("Senku", 30, "male")
        player.set_role("scientist")

        # create a new session.
        senku_session = SessionHandler(player, "doctor_lucas")

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
