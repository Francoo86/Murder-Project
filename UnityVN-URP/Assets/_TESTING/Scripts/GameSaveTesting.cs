using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

public class GameSaveTesting : MonoBehaviour
{
    public VNGameSave save;
    // Start is called before the first frame update
    void Start()
    {
        //save = VNGameSave.activeFile;
        //VNGameSave.activeFile = new VNGameSave();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) 
        {
            //tRY TO RETREIVE IT.
            save = VNGameSave.activeFile;
            ///Debug.Log($"PRINTING PLAYER NAME: {VNGameSave.activeFile.playerName}");
            //VNGameSave.activeFile.Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            /*
            try
            {
                save = VNGameSave.Load($"{FilePaths.gameSaves}1{VNGameSave.FILE_TYPE}", activateOnLoad: true);
                //VNGameSave.activeFile = FileManager.Load<VNGameSave>($"{FilePaths.gameSaves}1{VNGameSave.FILE_TYPE}");
                //VNGameSave.activeFile.Activate();
            }
            catch
            {
                Debug.LogError($"Error can't load data, the file is corrupt");
            }*/

        }
    }
}
