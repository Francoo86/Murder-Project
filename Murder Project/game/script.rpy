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

# PRO GAMER TIPS #
# default => Variable que se guarda por sesiones.
# define => Variable constante.

define aiko = Character("Aiko")



transform half_size:
    zoom 0.4

# El juego comienza aquí.

label start:
    # This needs to be kept across sessions.
    default info = PlayerInfo("Juan", 20, "Male", "Scientist")

    # load player data.
    default ply = SinglePlayer(info)

    show text "Iniciando..."
    pause 1

    python:
        inv = Inventory()

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

        player = SinglePlayer(info)

        # create a new session.
        senku_session = SessionHandler(player.info, "doctor_lucas")

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
