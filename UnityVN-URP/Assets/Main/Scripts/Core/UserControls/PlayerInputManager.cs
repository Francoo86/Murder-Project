using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("En verdad estoy llamando esto?");
            PromptAdvance();
        }
    }

    //Minuto 42
    public void PromptAdvance() {
        DialogController.Instance.OnUserPrompt_Next();
    }
}
