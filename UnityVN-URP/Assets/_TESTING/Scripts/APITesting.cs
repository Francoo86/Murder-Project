using System.Collections.Generic;
using UnityEngine;

public class APITesting : MonoBehaviour
{
    // Start is called before the first frame update
    public APIClient client;
    async void Start()
    {
        Dictionary <string, string> data = new Dictionary<string, string>
        {
            { "givenName", "Juan"},
            { "age", "26" },
            { "gender", "male"},
            { "role", "detective"},
        };

        client = new APIClient();
        //frickin works!!!!
        var session = await client.RequestCharacterSession("ana", data);

        AISessionHandler sess = new AISessionHandler("sujeto_5", client);

        PromptSender prompt = PromptSender.Instance;

        Debug.Log("injecting session to prompt object");
        prompt.InjectSession(sess);
        prompt.Talk("hey lad how are you doing?");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
