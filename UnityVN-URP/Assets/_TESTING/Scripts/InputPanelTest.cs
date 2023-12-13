using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanelTest : MonoBehaviour
{
    public PromptPanel promptPanel;

    void Start()
    {
        StartCoroutine(RunningPrompt());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RunningPrompt()
    {
        Character marcelo = CharacterController.Instance.CreateCharacter("Marcelo", true);
        yield return marcelo.Say("Preguntame cualquier cosa...");

        promptPanel.Show("Preguntale a Marcelo");

        while (promptPanel.IsWaitingOnUserInput)
            yield return null;

        string lastInput = promptPanel.LastInput;

        AISessionManager aiManager = new AISessionManager("ana", APIClientV2.Instance);
        CoroutinePrompt prompt = CoroutinePrompt.Instance;
        prompt.InjectSession(aiManager);

        yield return prompt.Talk(lastInput);
        yield return prompt.TestCharacter(marcelo);

        yield return null;
    }
}
