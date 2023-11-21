using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
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
        string session = await client.RequestCharacterSession("ana", data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
