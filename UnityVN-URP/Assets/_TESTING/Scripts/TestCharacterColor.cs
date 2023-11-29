using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterColor : MonoBehaviour
{
    private Character CreateCharacter (string name) => CharacterController.Instance.CreateCharacter(name);
    void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        SpriteCharacter Marcelo = CreateCharacter("Marcelo") as SpriteCharacter;
        SpriteCharacter Patricia = CreateCharacter("Patricia") as SpriteCharacter;

        Marcelo.Show();
        Patricia.Show();

        yield return null;
        Marcelo.SetPos(new Vector2(0.9f, 0.73f));
        //Patricia.SetPos(new Vector2(1, 0));

        Patricia.UnHighlight();
        yield return Marcelo.Say("Hola");

        Marcelo.UnHighlight();
        Patricia.Highlight();
        yield return Patricia.Say("hola bien y tu");

        yield return Patricia.Say("Yay!");
        yield return null;
    }

    void Update()
    {

    }
}
