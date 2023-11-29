using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace TESTING
{
*/
public class TextCharacter : Character
{
    public TextCharacter(string name, CharacterConfigData config) : base(name, config, prefab: null) { }
    /*
    void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        SpriteCharacter Marcelo = CreateCharacter("Marcelo");
        SpriteCharacter Patricia = CreateCharacter("Patricia");

        Marcelo.SetPosition(Vector2.zero);
        Patricia.SetPosition(new Vector2(1, 0));

        Patricia.UnHighlight();
        yield return Marcelo.Say("Hola");

        Marcelo.UnHighlight();
        Patricia.Highlight();
        yield return Patricia.Say("hola bien y tu");

        yield return Patrica.Say("Yay!");
        yield return null;
    }

    void Update()
    {

    }
    */
}
/*
}
*/