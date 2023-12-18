using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;


/// <summary>
/// Placeholder character for those characters that doesn't show nothing representative on the screen.
/// </summary>
public class TextCharacter : Character
{
    //Este personaje es aquel que aparece sin imagen ni nada.
    public TextCharacter(string name, CharacterConfigData config) : base(name, config, prefab: null) { }
}