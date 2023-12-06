using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAudioExtension : CommandDBExtension
{
    //Para efectos de sonido.
    private static string[] PARAM_SFX = new string[]{"-s", "-sound", "-sfx"};
    private static string[] PARAM_VOLUME = new string[]{ "-v", "-vol", "-volume" };
    private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
    private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

    //Para musica.
    private static string[] PARAM_CHANNEL = new string[] { "-chan", "-c", "-channel" };
    private static string[] PARAM_INMEDIATE = new string[] { "-i", "-inmediate" };
    private static string[] PARAM_START_VOLUME = new string[] { "-sv", "startvolume" };
    private static string[] PARAM_SONG = new string[] { "-s", "-song", "-source" };
    private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };

    new public static void Extend(CommandDB commandDb)
    {
        commandDb.AddCommand("playsfx", new Action<string[]>(PlaySFX));
        commandDb.AddCommand("playsong", new Action<string[]>(PlaySong));
        commandDb.AddCommand("playambience", new Action<string[]>(PlayAmbience));

        commandDb.AddCommand("stopsfx", new Action<string>(StopSFX));
        commandDb.AddCommand("stopsong", new Action<string>(StopSong));
        commandDb.AddCommand("stopambience", new Action<string>(StopAmbience));
    }

    private static void StopSFX(string data)
    {
        AudioController.Instance.StopSoundEffects(data);
    }

    private static void PlaySFX(string[] data)
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
    }

    private static void PlaySong(string[] data)
    {
        string filepath;
        int channel;

        var parameters = ConvertToParams(data);

        parameters.TryGetValue(PARAM_SONG, out filepath);
        filepath = FilePaths.GetPathToResource(FilePaths.ResourcesMusic, filepath);

        parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultVal: 1);

        PlayTrack(filepath, channel, parameters);
    }

    private static void PlayAmbience(string[] data)
    {
        string filepath;
        int channel;

        var parameters = ConvertToParams(data);

        parameters.TryGetValue(PARAM_AMBIENCE, out filepath);
        filepath = FilePaths.GetPathToResource(FilePaths.ResourcesAmbience, filepath);

        parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultVal: 0);

        PlayTrack(filepath, channel, parameters);
    }

    private static void PlayTrack(string filepath, int channel, CommandParameters parameters)
    {
        bool loop;
        float volCap, startVol, pitch;

        //Obtener los datos principales de un track.
        parameters.TryGetValue(PARAM_VOLUME, out volCap, defaultVal: 1f);
        parameters.TryGetValue(PARAM_START_VOLUME, out startVol, defaultVal: 0f);
        parameters.TryGetValue(PARAM_PITCH, out pitch, defaultVal: 1f);

        //Loopear o hacer que se ejecute de inmediato.
        parameters.TryGetValue(PARAM_LOOP, out loop, defaultVal: true);

        AudioClip audioTrack = Resources.Load<AudioClip>(filepath);

        if (audioTrack == null)
        {
            return;
        }

        //I forgor that this method was overloaded.
        AudioController.Instance.PlayTrack(audioTrack, channel, loop, startVol, volCap, filepath);
    }

    private static void StopSong(string data)
    {
        if (data == string.Empty)
            StopTrack("1");
        else
            StopTrack(data);
    }

    private static void StopAmbience(string data)
    {
        if (data == string.Empty)
            StopTrack("0");
        else
            StopTrack(data);
    }

    private static void StopTrack(string data)
    {
        if (int.TryParse(data, out int channel))
        {
            AudioController.Instance.StopTrack(channel);
        }
        else
            AudioController.Instance.StopTrack(data);
    }
}
