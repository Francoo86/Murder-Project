using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISAULNOVEL;

public class GameSaveTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VNGameSave.activeFile = new VNGameSave();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) 
        {
            VNGameSave.activeFile.Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            VNGameSave.activeFile.Load();
        }
    }
}