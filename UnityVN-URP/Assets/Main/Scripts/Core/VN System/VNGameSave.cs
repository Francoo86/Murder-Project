using System.Collections.Generic;
using UnityEngine;
using History;
using System.Linq;
using System;

namespace VISUALNOVEL
{
    [System.Serializable]

    public class VNGameSave
    {
        public static VNGameSave activeFile = null;
        public const string FILE_TYPE = ".vns";
        public const string SCREENSHOT_FILE_TYPE = ".jpg";
        public const bool ENCRYPY_FILE = false;
        public const float SCREENSHOT_DOWNSCALE = 1;

        public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
        public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";

        public string playerName;
        public int slotNumber = 1;

        public bool newGame = true;
        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLog;

        public VN_VariableData[] variables;

        public string timestamp;

        public void Save() 
        {
            newGame = false;

            activeState = HistoryState.Capture();
            historyLog = HistoryManager.Instance.history.ToArray();
            activeConversations = GetConversationData();
            variables = GetVariableData();

            timestamp = DateTime.Now.ToString("dd-MM-yy HH:mm:ss");

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON);

            ScreenshootMaster.CaptureScreenshot(VNManager.Instance.mainCamera, Screen.width, Screen.height, SCREENSHOT_DOWNSCALE, screenshotPath);
        }

        /// <summary>
        /// Applies the save file to the screen.
        /// </summary>
        public void Activate() 
        {
            if (activeState != null)
                activeState.Load();
            
            HistoryManager.Instance.history = historyLog.ToList();
            //Wrapper method for refactoring this stuff.
            HistoryManager.Instance.ResetLogManager();
            //HistoryManager.Instance.logManager.Clear();
            //HistoryManager.Instance.logManager.Rebuild();

            SetVariableData();
            SetConversationData();
            
            //Esconder el cursor.
            //DialogController.Instance.prompt.Hide();
        }

        //ESTA FUNCION WEBEA CON LAS VARIABLES.
        //This function it shouldn't set the activeFile instantly.
        public static VNGameSave Load(string filePath, bool activateOnLoad = false)
        {
            VNGameSave save = FileManager.Load<VNGameSave>(filePath);
            activeFile = save;

            if (activateOnLoad)
                save.Activate();
            return save;  
        }

        public string[] GetConversationData()
        {
            List<string> retData = new List<string>();

            var conversations = DialogController.Instance.convManager.GetConversationQueue();

            for (int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if (conversation.file != string.Empty)
                {
                    var compressedData = new VN_ConversationDataCompressed();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endIndex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }
                else 
                {
                    var fullData = new VN_ConversationData();
                    fullData.conversation = conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }
                retData.Add(data);
            }
            return retData.ToArray();
        }

        private void SetConversationData()
        {
            for(int i = 0; i < activeConversations.Length; i++) 
            {
                try
                {
                    string data = activeConversations[i];
                    Conversation conversation = null;

                    var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                    if (fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                    {
                        conversation = new Conversation(fullData.conversation, fullData.progress);
                    }
                    else
                    {
                        var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                        if (compressedData != null && compressedData.fileName != string.Empty)
                        {
                            TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);
                            int count = compressedData.endIndex - compressedData.startIndex;
                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count+1).ToList();
                            conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                        }
                        else
                        {
                            Debug.LogError($"Unable to reload conversation from VNGameSave data '{data}'");
                        }
                    }

                    if (conversation != null && conversation.GetLines().Count > 0) 
                    {
                        if (i == 0)
                            DialogController.Instance.convManager.StartConversation(conversation);
                        else
                            DialogController.Instance.convManager.Enqueue(conversation);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error while extracting saved conversation data {e}");
                    continue;
                }
            }
        }

        private VN_VariableData[] GetVariableData()
        {
            List<VN_VariableData> retData = new List<VN_VariableData>();

            foreach (var database in VariableStore.databases.Values)
            {
                foreach (var variable in database.variables)
                {
                    VN_VariableData variableData = new VN_VariableData();
                    variableData.name = $"{database.name}.{variable.Key}";
                    string val = $"{variable.Value.Get()}";
                    variableData.value = val;
                    //Debug.Log($"<color=#00FF00>Retreiving the variable: {variableData.name} val: {variableData.value}</color>");
                    variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString(); 
                    retData.Add(variableData);
                }
            }
            return retData.ToArray();
        }

        private void SetVariableData()
        {
            foreach (var variable in variables)
            {
                string val = variable.value;

                switch (variable.type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(val, out bool b_val))
                        {
                            VariableStore.TrySetValue(variable.name, b_val);
                            continue;
                        }
                        break;

                    case "System.Int32":
                        if (int.TryParse(val, out int i_val))
                        {
                            VariableStore.TrySetValue(variable.name, i_val);
                            continue;
                        }
                        break;

                    case "System.Single":
                        if (float.TryParse(val, out float f_val))
                        {
                            VariableStore.TrySetValue(variable.name, f_val);
                            continue;
                        }
                        break;
                    case "System.Double":
                        if(float.TryParse(val, out float d_val))
                        {
                            VariableStore.TrySetValue(variable.name, d_val);
                            continue;
                        }
                        break;
                    case "System.String":
                        VariableStore.TrySetValue(variable.name, val);
                        continue;
                }

                Debug.LogError($"Could not interpret variable type {variable.name} = {variable.type}");
            }
        }
    }
}