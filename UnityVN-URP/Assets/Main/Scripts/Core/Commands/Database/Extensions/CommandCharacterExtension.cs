using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandCharacterExtension : CommandDBExtension
{
    //Activar personajes y eso.
    private static string[] ENABLED_CHARACTER = new string[] { "-e", "-enabled" };
    private static string[] INMEDIATE_APPEARING = new string[] { "-i", "-inmediate" };
    //Posiciones.
    private static string XPOS = "-x";
    private static string YPOS = "-y";
    private static string[] SPEED_PARAM = new string[]{ "-s", "-speed" };
    private static string SMOOTH_PARAM = "-smoothness";

    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
        commandDB.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
        commandDB.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));
    }

    private static void CreateCharacter(string[] data)
    {
        string charName = data[0];
        bool enabled = false;
        bool inmediate = false;
        Character character = CharacterController.Instance.CreateCharacter(charName, enabled);

        var parameters = ConvertToParams(data);
        parameters.TryGetValue(ENABLED_CHARACTER, out enabled, false);
        parameters.TryGetValue(INMEDIATE_APPEARING, out inmediate, false);

        Debug.Log($"Creating from command {charName}");

        if (!enabled) return;

        if (inmediate)
            character.IsVisible = true;
        else
            character.Show();
    }

    //Trying.
    private static IEnumerator MoveCharacter(string[] data)
    {
        string charName = data[0];

        Character character = CharacterController.Instance.GetCharacter(charName);

        if(character == null) yield break;

        float x = 0, y = 0;
        float speed = 0;
        bool smooth = false;
        bool inmediate = false;

        var parameters = ConvertToParams(data);
        //Obtener posicion en X.
        parameters.TryGetValue(XPOS, out x);
        //En Y.
        parameters.TryGetValue(YPOS, out y);
        //Ver si se mueve de inmediato.
        parameters.TryGetValue(SPEED_PARAM, out speed, defaultVal: 1);
        //Queremos suavidad?
        parameters.TryGetValue(SMOOTH_PARAM, out smooth, false);
        //Altoque?
        parameters.TryGetValue(INMEDIATE_APPEARING, out inmediate, false);

        Vector2 pos = new Vector2(x, y);
        if (inmediate)
            character.SetPos(pos);
        else
            CommandController.Instance.AddTerminationActionToActualProcess(() => { character.SetPos(pos); });
            yield return character.MoveToPosition(pos, speed, smooth);

        //yield return null;
    }

    private static IEnumerator ShowAll(string[] data)
    {
        List<Character> allCharacters = new List<Character>();
        bool inmediate = false;

        foreach (string character in data)
        {
            Character currentCharacter = CharacterController.Instance.GetCharacter(character, create: false);

            if(currentCharacter != null)
                allCharacters.Add(currentCharacter);
        }

        if(allCharacters.Count == 0)
        {
            yield break;
        }

        var parameters = ConvertToParams(data);
        parameters.TryGetValue(INMEDIATE_APPEARING, out inmediate, false);

        foreach(Character character in allCharacters)
        {
            if (!inmediate)
                character.Hide();
            else
                character.IsVisible = false;
        }

        if (!inmediate)
        {
            while(allCharacters.Any(c => c.IsHiding))
            {
                yield return null;
            }
        }
    }

    public static void HideAll(string[] data)
    {

    }
}
