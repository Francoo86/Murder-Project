using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

public class GameSaveTesting : MonoBehaviour
{
    public VNGameSave save;
    public string characterName;
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
        else if (Input.GetKeyDown(KeyCode.K))
        {
            
            ConversationManager Controller = DialogController.Instance.convManager;
            Debug.Log($"<color=#104901>{Controller.conversation.GetCurrentLine()}</color>");
            
            List<string> linesToAppend = new List<string>
            {
                "",
                "Lua is a lightweight scripting language designed for embedded systems and general-purpose programming.",
                "Known for its simplicity and efficiency, Lua is often used in game development and various applications.",
                "Lua features a minimalistic syntax, making it easy to integrate into existing projects as a scripting language.",
                "Lua supports extensibility, allowing seamless integration with C and other languages.",
                "Beyond gaming, Lua is used in diverse domains such as scripting for software applications and configuration files."
            };
            List<string> conversationLines = Controller.conversation.GetLines();
            int progress = Controller.conversation.GetProgress();

            for (int i = linesToAppend.Count - 1; i >= 0; i--)
            {
                string line = $"{characterName} \"{linesToAppend[i]}\"";
                conversationLines.Insert(progress, line);
            }

            //DialogController.Instance.dialogContainer.dialogText.text = linesToAppend[linesToAppend.Count - 1];
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ConversationManager convManager = DialogController.Instance.convManager;

            Debug.Log($"<color=#FFC0CB>{convManager.conversation.GetCurrentLine()}</color>");
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
