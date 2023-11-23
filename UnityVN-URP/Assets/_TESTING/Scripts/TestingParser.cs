using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SendFileToParser();
    }

    // Update is called once per frame
    void SendFileToParser()
    {
        List<string> lines = FileManager.ReadTextFile("textomomento.txt");

        DialogController.Instance.Say(lines);

    }
}
