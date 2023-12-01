using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingDialog : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTalking());
    }

    IEnumerator StartTalking()
    {
        SpriteCharacter charPedro = CharacterController.Instance.CreateCharacter("Marcelo") as SpriteCharacter;
        Character alfonso = CharacterController.Instance.CreateCharacter("Patricia");
        List<string> lines = new List<string>()
        {
            "XDDDD",
            "AYUDA PORFA",
            "A PASAR TIS",
            "AAAAXDDDDDDDDDDDDDDDDD",
        };

        charPedro.SetDialogColor(Color.red);

        yield return new WaitForSeconds(1f);
        yield return charPedro.Hide();
        yield return new WaitForSeconds(1f);
        yield return charPedro.Show();

        yield return new WaitForSeconds(1f);
        Sprite pensativo = charPedro.GetSprite("feliz");
        Debug.Log("Transitioning to pensativo");
        charPedro.TransitionSprite(pensativo);
        //charPedro.SetSprite(pensativo);
        //Wacky ahh pos.
        charPedro.SetPos(new Vector2(0.5f, 0.4f));

        yield return charPedro.Say(lines);


        charPedro.SetNameColor(Color.cyan);
        charPedro.SetDialogColor(Color.green);

        yield return charPedro.Say(lines);

        yield return alfonso.Say(lines);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
