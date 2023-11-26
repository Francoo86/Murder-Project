using UnityEngine;

[CreateAssetMenu(fileName = "Character Configuration File", menuName = "Dialog System/Character Configuration File")]
public class CharacterConfig : ScriptableObject
{
    public CharacterConfigData[] characters;
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