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

        yield return Marcelo.Show();
        Patricia.Show();

        Vector2 vec = new Vector2(0.5f, 0.38f);
        yield return Marcelo.MoveToPosition(vec);
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
