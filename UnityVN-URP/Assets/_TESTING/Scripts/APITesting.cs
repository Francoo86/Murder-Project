using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using CHARACTERS;

public class APITesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestPedro());
       
        /*
        AISessionHandler sess = new AISessionHandler("sujeto_5", client);
        await sess.RequestNewSession();

        Debug.Log($"Actual session: {sess.PlayerSessionId}");

        //PromptSender prompt = PromptSender.Instance;

        Debug.Log("injecting session to prompt object");
        prompt.InjectSession(sess);

        prompt.Talk("hey lad how are you doing?");

        initialized= true;*/
    }

    
    IEnumerator TestPedro()
    {
        SpriteCharacter charPedro = CharacterController.Instance.CreateCharacter("Marcelo") as SpriteCharacter;
        Character textChar = CharacterController.Instance.CreateCharacter("Player");
        yield return charPedro.Show();

        Sprite test = charPedro.GetSprite("sorprendido");
        Debug.Log(test);
        yield return new WaitForSeconds(2);
        charPedro.SetSprite(test);
        
        //Dependency injection perhaps.
        AISessionManager aiManager = new AISessionManager("ana");
        CoroutinePrompt prompt = CoroutinePrompt.GetInstance();
        prompt.InjectSession(aiManager);

        string[] lines = new string[5]
        {
            "Hey how are you?",
            "Did i asked you that how you are?",
            "Do you like misteries?",
            "Do you know about ancient history?",
            "Are we living on the matrix?",
        };

        //Make the character talk in the coroutines.
        for(int i =  0; i < lines.Length; i++)
        {
            yield return textChar.Say(lines[i]);
            yield return prompt.Talk(lines[i]);
            yield return prompt.TestCharacter(charPedro);
        }
    }

    IEnumerator TestWeb()
    {
        UnityWebRequest uwRequest = UnityWebRequest.Get("https://pokeapi.co/api/v2/pokemon/ditto");

        yield return uwRequest.SendWebRequest();

        var text = uwRequest.downloadHandler.text;
        Debug.Log(text);
    }
    // Update is called once per frame
    void Update()
    {
        /*
        if (initialized)
        {
            StartCoroutine(TestPedro());
            initialized = false;
        }*/
    }
}
