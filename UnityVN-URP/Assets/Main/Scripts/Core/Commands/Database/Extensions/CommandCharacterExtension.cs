using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCharacterExtension : CommandDBExtension
{
    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
    }

    public static IEnumerator ShowAll(string[] data)
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
    }

    public static void HideAll(string[] data)
    {

    }
}
