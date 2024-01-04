using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using CHARACTERS;
using System.Linq;

/// <summary>
/// The Character Manager/Controller that handles the logic of the characters across the game.
/// </summary>
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
    /// <summary>
    /// Creates a characters based on their resources and checks if it was defined on the configuration.
    /// </summary>
    /// <param name="charName">Character name.</param>
    /// <param name="revealAfterCreated">Marks to be revealed on the next frame.</param>
    /// <returns>The created character.</returns>
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
    //private CHARACTER_INFO GetCharacterInfo(string characterName)
    //{
    //    CHARACTER_INFO result = new CHARACTER_INFO;

    //    string[] nameData = characterName.Split(CHARACTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
    //    result.name = nameData[0];
    //    result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

    //    result.config = config.GetConfig(result.castingName);

    //    result.prefab = GetPrefabForCharacter(result.castingname);

    //    result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);

    //    return result;

    //}
    

    /// <summary>
    /// Retrieves a character from the stored characters list.
    /// </summary>
    /// <param name="charName">The character name.</param>
    /// <param name="create">Forces the creation of the character if it doesn't exists.</param>
    /// <returns>The character.</returns>
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

    /// <summary>
    /// Checks if the controller has the requested character.
    /// </summary>
    /// <param name="charName">The character name.</param>
    /// <returns>Wether the character exists or not.</returns>
    public bool HasCharacter(string charName) => storedChars.ContainsKey(charName.ToLower());

    /// <summary>
    /// Obtains the character configuration data (defined in the Unity file) based on character name.
    /// </summary>
    /// <param name="charName">The character name.</param>
    /// <returns>The character configuration data.</returns>
    public CharacterConfigData GetCharacterConfig(string charName, bool useOriginal = false)
    {
        if (!useOriginal)
        {
            Character character = GetCharacter(charName);

            if (character != null)
                return character.config;
        }

        return config.GetConfig(charName);
    }

    /// <summary>
    /// Creates a character based on their model, primarily the configuration associated with it.
    /// </summary>
    /// <param name="characterModel">The character model.</param>
    /// <returns>The created character.</returns>
    private Character CreateCharacterFromModel(CharacterModel characterModel)
    {
        CharacterConfigData data = characterModel.config;

        switch (data.charType)
        {
            case Character.CharacterType.Text:
                return new TextCharacter(characterModel.name, data);
            case Character.CharacterType.Sprite:
                return new SpriteCharacter(characterModel.name, data, characterModel.prefab, characterModel.baseCharacterFolder);
            default:
                break;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the character model object of a character based on its name.
    /// </summary>
    /// <param name="charName">The character name.</param>
    /// <returns>The character model object.</returns>
    private CharacterModel GetCharacterModel(string charName)
    {
        CharacterModel model = new CharacterModel();
        model.name = charName;
        model.config = config.GetConfig(charName);
        model.prefab = LookupPrefab(charName);
        model.baseCharacterFolder = FormatCharacterPath(characterPath, charName);
        return model;
    }

    /// <summary>
    /// Internal method that searches a prefab resource asociated with a character.
    /// </summary>
    /// <param name="charName">The character name.</param>
    /// <returns>The prefab GameObject loaded.</returns>
    private GameObject LookupPrefab(string charName)
    {
        string resPath = FormatCharacterPath(characterPrefabPath, charName);
        return Resources.Load<GameObject>(resPath);
    }

    /// <summary>
    /// Internal method that retrieves the character image path to get their expressions.
    /// It is a folder associated with the character name.
    /// </summary>
    /// <param name="path">The character path.</param>
    /// <param name="charName">The character name.</param>
    /// <returns>The character path.</returns>
    private string FormatCharacterPath(string path, string charName) => path.Replace(CHARACTER_ID, charName);

}

/// <summary>
/// Class that holds all info that composes a character, like the image, the name and the prefab (resources to be loaded).
/// </summary>
class CharacterModel {
    public string name;
    public CharacterConfigData config = null;
    public GameObject prefab;
    public string baseCharacterFolder = "";
}