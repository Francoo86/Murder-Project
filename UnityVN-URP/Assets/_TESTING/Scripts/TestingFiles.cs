using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingFiles : MonoBehaviour
{
    private string scriptedSeqFile = "ScriptedDialog.txt";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Run());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Run() {
        List <string> lines = FileManager.ReadTextFile(scriptedSeqFile);

        foreach (string line in lines)
        {
            Debug.Log(line);
        }

        yield return null;
    }
}
