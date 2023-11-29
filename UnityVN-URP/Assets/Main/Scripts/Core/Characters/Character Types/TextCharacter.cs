using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace TESTING
{
*/
public class TextCharacter : Character
{
    //Este personaje es aquel que aparece sin imagen ni nada.
    public TextCharacter(string name, CharacterConfigData config) : base(name, config, prefab: null) { }
}
/*
}
*/