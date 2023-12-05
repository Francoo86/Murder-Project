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

    private static void PlaySFX(string[] data)
    {
        string filepath;
        float volume, pitch;
        bool loop;

        //var param = ConvertDataToParameters(data);
    }
}
