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
        ChoicePanel choicePanel = ChoicePanel.Instance;
        string[] choices = new string[5]
{
            "Lore ipsum bla blaaaaaaaaaaa",
            "Choice 2",
            "Choice 3",
            "Choice 4",
            "Choice 5",
};

        choicePanel.Show("Do you like minecraft?", choices);

        while (choicePanel.IsWaitingOnUserChoice)
            yield return null;

        var decision = choicePanel.LastDecision;

        Debug.Log($"Made choice {decision.answerIndex} '{decision.choices[decision.answerIndex]}'");
        /*
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

        yield return null;*/

        //choicePanel.Show("Do you like minecraft?", choices);
    }
}
