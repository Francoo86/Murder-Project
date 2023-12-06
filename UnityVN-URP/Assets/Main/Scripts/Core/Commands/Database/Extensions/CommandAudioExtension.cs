using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAudioExtension : CommandDBExtension
{
    private static string[] PARAM_SFX = new string[]{"-s", "-sound", "-sfx"};
    private static string[] PARAM_VOLUME = new string[]{ "-v", "-vol", "-volume" };
    private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
    private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

    new public static void Extend(CommandDB commandDb)
    {
        commandDb.AddCommand("playsfx", new Action<string[]>(PlaySFX));
    }

    public static void PlaySFX(string[] data)
    {
        string filepath;
        float volume, pitch;
        bool loop;

        //finally?!!?!!?!?
        var parameters = ConvertToParams(data);

        //Directorio.
        parameters.TryGetValue(PARAM_SFX, out filepath);

        //Volumen.
        parameters.TryGetValue(PARAM_VOLUME, out volume, 1f);

        //Pitch del sonido (creo que no está implementado).
        parameters.TryGetValue(PARAM_PITCH, out pitch, 1f);

        //Se loopea.
        parameters.TryGetValue(PARAM_LOOP, out loop, false);

        AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.ResourcesSFX, filepath));

        if (sound == null) return;

        AudioController.Instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop);

        //Obtener el directorio.

    }
}
