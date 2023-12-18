using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using CHARACTERS;

/// <summary>
/// Serializable class that associates the character configuration data, to make a character.
/// This is where CharacterController loads info about a character.
/// </summary>
[System.Serializable]
public class CharacterConfigData
{
    //TODO: Fix coupling aAAAAAAAAAAAAAA.
    public string name;
    //Como se mostrara el personaje en pantalla, algo así como Marcelo -> Alcalde.
    public string alias;
    public Character.CharacterType charType;

    public Color nameCol;
    public Color diagCol;

    public TMP_FontAsset nameFont;
    public TMP_FontAsset diagFont;


    /// <summary>
    /// Gets a full copy of the Character configuration, to be used for saving the character in the files.
    /// </summary>
    /// <returns>The new copy.</returns>
    public CharacterConfigData Copy() {
        CharacterConfigData copy = new CharacterConfigData();

        copy.name = name;
        copy.alias = alias;
        copy.charType = charType;
        copy.nameFont = nameFont;
        copy.diagFont = diagFont;

        //Los colores al ser un struct siempre usamos su referencia.
        copy.nameCol = new Color(nameCol.r, nameCol.g, nameCol.b, nameCol.a);
        copy.diagCol = new Color(diagCol.r, diagCol.g, diagCol.b, diagCol.a);

        return copy;
    }

    public static Color DefaultColor => DialogController.Instance.Config.defaultTextColor;
    public static TMP_FontAsset DefaultFont => DialogController.Instance.Config.defaultFont;

    /// <summary>
    /// Gets the default character configuration as a new CharacterConfigData.
    /// </summary>
    public static CharacterConfigData Default
    {
        get
        {
            CharacterConfigData data = new CharacterConfigData();

            data.name = "";
            data.alias = "";
            data.charType = Character.CharacterType.Text;
            //La fuente por defecto de unity.
            data.nameFont = DefaultFont;
            data.diagFont = DefaultFont;

            data.nameCol = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.b, DefaultColor.a);
            data.diagCol = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.b, DefaultColor.a);


            return data;
        }
    }
}
