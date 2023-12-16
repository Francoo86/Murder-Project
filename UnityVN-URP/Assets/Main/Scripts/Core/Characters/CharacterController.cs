using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using CHARACTERS;
using System.Linq;

public class CharacterController : MonoBehaviour
{
    //Estos objetos de tipo controlador solo existen una sola vez en el "sistema".
    public static CharacterController Instance { get; private set; }
    public Character[] allCharacters => storedChars.Values.ToArray();
    private Dictionary<string, Character> storedChars = new Dictionary<string, Character>();
    private CharacterConfig config => DialogController.Instance.Config.charConfigAsset;

    //Constantes para poder realizar un lookup a un personaje.
    //En español para las cadenas para poder proporcionar flexibilidad.
    private const string CHARACTER_ID = "<personaje>";
    private string characterPath => $"Characters/{CHARACTER_ID}";
    private string characterPrefabPath => $"Characters/{CHARACTER_ID}/Personaje - [{CHARACTER_ID}]";
    [SerializeField] private RectTransform charPanel = null;
    public RectTransform CharacterPanel => charPanel;

    private void Awake()
    {
        Instance = this; 
    }
    
    //TODO: Make factory method with this.
    public Character CreateCharacter(string charName, bool revealAfterCreated = false) {
        if (storedChars.ContainsKey(charName.ToLower()))
        {
            Debug.LogError($"The character {charName} already exists, the character was not created.");
            return null;
        }

        CharacterModel characterModel = GetCharacterModel(charName);
        Character character = CreateCharacterFromModel(characterModel);
        storedChars.Add(charName.ToLower(), character);

        if (revealAfterCreated)
            character.Show();

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

    public bool HasCharacter(string charName) => storedChars.ContainsKey(charName.ToLower());

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
            //case Character.CharacterType.SpriteSheet:
                return new SpriteCharacter(characterModel.name, data, characterModel.prefab, characterModel.baseCharacterFolder);
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
        model.prefab = LookupPrefab(charName);
        model.baseCharacterFolder = FormatCharacterPath(characterPath, charName);
        return model;
    }

    private GameObject LookupPrefab(string charName)
    {
        string resPath = FormatCharacterPath(characterPrefabPath, charName);
        return Resources.Load<GameObject>(resPath);
    }

    private string FormatCharacterPath(string path, string charName) => path.Replace(CHARACTER_ID, charName);

}

class CharacterModel {
    public string name;
    public CharacterConfigData config = null;
    public GameObject prefab;
    public string baseCharacterFolder = "";
}