# defined client for TIS workspace.
define ai_dynamic = Character("AIBase")

init python:
    from lib.inworld_api import PromptSender, AISessionHandler
    from lib.player import PlayerModel
    from lib.inworld_connection import InworldAPIClient

    # unique instance of playermodel.
    info = PlayerModel("Luis", 35, "Male", "Detective")
    # uses the default .env if not provided.
    client = InworldAPIClient()

    # save this things.
    CHARACTERS = {
        #"ana": "Ana",
        #"sujeto4": "Anastasia",
        "sujeto8-dvv3d": "Jacinto",
        "sujeto3": "Juan",
        "sujeto6": "Lucia",
        # pun intended.
        "sejeto0": "Luis",
        "sujeto_5": "Marcelo",
        "sujeto9": "Patricia",
        "sujeto2": "Pedro",
        "sujeto7": "Santiago"
    }

    sessions = {}
    saved_tuples = []

    for key in CHARACTERS:
        # save character onto sessions.
        real_name = CHARACTERS[key]
        sessions[real_name] = AISessionHandler(info, client, key)
        saved_tuples.append((f"Talk with {real_name}", real_name))

    def get_session_by_name(name : str) -> AISessionHandler:
        sess = sessions.get(name, None)

        if sess is not None:
            return sess

        return "Dummy"