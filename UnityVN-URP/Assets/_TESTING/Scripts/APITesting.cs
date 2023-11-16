using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APITesting : MonoBehaviour
{
    // Start is called before the first frame update
    public APIClient client = new APIClient();
    void Start()
    {
        Dictionary <string, string> data = new Dictionary<string, string>
        {
            { "givenName", "Juan"},
            { "age", "26" },
            { "gender", "male"},
            { "role", "detective"},
        };

        client.SendSimpleText("hello", "sujeto3", data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
