using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

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
        yield return new WaitForSeconds(1);
        yield return Marcelo.Flip(0.3f);
       
        yield return Patricia.FaceRight(immediate:true);
        yield return Patricia.FaceLeft(immediate:true);

        NUEVO PARA HACER EL TESTEO XD
*/