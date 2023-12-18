using UnityEngine;

/// <summary>
/// Character configuration class used by Unity to create custom characters.
/// </summary>
[CreateAssetMenu(fileName = "Character Configuration File", menuName = "Dialog System/Character Configuration File")]
public class CharacterConfig : ScriptableObject
{
    public CharacterConfigData[] characters;
    /// <summary>
    /// Gets the config associated by the character.
    /// The character holds info like, the font, the type of character (sprite, text, spritesheet), the name, the color.
    /// </summary>
    /// <param name="characterName">The character to obtain info.</param>
    /// <returns>The character data, otherwise it returns the default one (Narrator).</returns>
    public CharacterConfigData GetConfig(string characterName) { 
        characterName = characterName.ToLower();

        for(int i  = 0; i < characters.Length; i++)
        {
            CharacterConfigData data = characters[i];
            if(string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
            {
                return data;
            }        
        }

        return CharacterConfigData.Default;
    }
}
