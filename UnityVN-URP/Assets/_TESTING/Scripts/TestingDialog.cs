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
        Character charPedro = CharacterController.Instance.CreateCharacter("Pedro");
        Character alfonso = CharacterController.Instance.CreateCharacter("Alfonso");
        List<string> lines = new List<string>()
        {
            "XDDDD",
            "AYUDA PORFA",
            "A PASAR TIS",
            "AAAAXDDDDDDDDDDDDDDDDD",
        };

        charPedro.SetDialogColor(Color.red);

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
