using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //Estos objetos de tipo controlador solo existen una sola vez en el "sistema".
    public static CharacterController Instance { get; private set; }
    private Dictionary<string, Character> storedChars = new Dictionary<string, Character>();
    private CharacterConfig config => DialogController.Instance.Config.charConfigAsset;

    private void Awake()
    {
        Instance = this; 
    }
    
    //TODO: Make factory method with this.
    public Character CreateCharacter(string charName) {
        if (storedChars.ContainsKey(charName.ToLower()))
        {
            Debug.LogError($"The character {charName} already exists, the character was not created.");
            return null;
        }

        CharacterModel characterModel = GetCharacterModel(charName);
        Character character = CreateCharacterFromModel(characterModel);
        storedChars.Add(charName.ToLower(), character);

        return character;
    }

    public Character GetCharacter(string charName, bool create = false) {
        if (storedChars.ContainsKey(charName.ToLower()))
        {
            return storedChars[charName.ToLower()];
        }
        else if(create)
        {
            return CreateCharacter(charName);
        }

        return null;
    }

    public CharacterConfigData GetCharacterConfig(string charName)
    {
        return config.GetConfig(charName);
    }
    private Character CreateCharacterFromModel(CharacterModel characterModel)
    {
        CharacterConfigData data = characterModel.config;

        switch (data.charType)
        {
            case Character.CharacterType.Text:
                return new TextCharacter(characterModel.name, data);
            case Character.CharacterType.Sprite:
                return new SpriteCharacter(characterModel.name, data);
            case Character.CharacterType.SpriteSheet:
                return new SpriteCharacter(characterModel.name, data);
            default:
                break;
        }

        return null;
    }

    private CharacterModel GetCharacterModel(string charName)
    {
        CharacterModel model = new CharacterModel();
        model.name = charName;
        model.config = config.GetConfig(charName);
        return model;
    }

}

class CharacterModel {
    public string name;
    public CharacterConfigData config = null;
}