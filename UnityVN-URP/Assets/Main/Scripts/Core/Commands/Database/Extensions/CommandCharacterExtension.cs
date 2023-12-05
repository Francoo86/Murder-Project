using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandCharacterExtension : CommandDBExtension
{
    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
        commandDB.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
    }

    public static void CreateCharacter(string[] data)
    {
        string charName = data[0];
        bool enabled = false;

        var parameters = ConvertToParams(data);
        parameters.TryGetValue(new string[] {"-e", "-enabled"}, out enabled, false);

        Debug.Log($"Creating from command {charName}");

        CharacterController.Instance.CreateCharacter(charName, enabled);
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
        parameters.TryGetValue(new string[] {"-i", "-inmediate"}, out inmediate, false);

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
