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

    private Character CreateCharacterFromModel(CharacterModel characterModel)
    {
        CharacterConfigData data = characterModel.config;

        if(data.charType == Character.CharacterType.Text)
        {
            return new TextCharacter(characterModel.name);
        }

        if (data.charType == Character.CharacterType.Sprite || data.charType == Character.CharacterType.SpriteSheet) {
            return new SpriteCharacter(characterModel.name);
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