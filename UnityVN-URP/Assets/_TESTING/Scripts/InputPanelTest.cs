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
        Conversation conversation = null;

        if (Input.GetKeyDown(KeyCode.K))
        {
            List<string> cSharpTopics = new List<string>
            {
                "LINQ simplifies data querying in C#.",
                "Asynchronous programming with async/await improves responsiveness.",
                "Object-oriented programming (OOP) is a fundamental concept in C#.",
                "C# is widely used for building Windows applications.",
                "Generics in C# provide type-safe data structures and algorithms."
            };

            conversation = new Conversation(cSharpTopics);
            DialogController.Instance.convManager.Enqueue(conversation);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            List<string> luaTopics = new List<string>
            {
                "Lua is a lightweight and embeddable scripting language.",
                "Coroutines in Lua enable cooperative multitasking.",
                "Lua is often used in game development for scripting.",
                "Table data structure in Lua is versatile and powerful.",
                "Lua has a simple and consistent syntax, making it easy to learn."
            };

            conversation = new Conversation(luaTopics);
            DialogController.Instance.convManager.EnqueuePriority(conversation);
        }
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

        List<string> foodStatements = new List<string>
        {
            "Pizza is the perfect combination of cheese, sauce, and crust.",
            "Sushi is an art form with its delicate balance of flavors and textures.",
            "Chocolate is the answer, no matter what the question is.",
            "Tacos are a portable and delicious way to enjoy a variety of flavors.",
            "Freshly baked bread has a comforting aroma that fills the kitchen."
        };

        yield return DialogController.Instance.Say(foodStatements);

        yield return DialogController.Instance.Hide(1f, false);
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
