using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using History;
using System.Linq;

namespace VISAULNOVEL
{
    [System.Serializable]

    public class VNGameSave
    {
        public static VNGameSave activeFile = null;
        public const string FILE_TYPE = ".vns";
        public const string SCREENSHOT_FILE_TYPE = ".jpg";
        public const bool ENCRYPY_FILE = false;

        public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
        public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";

        public string playerName;
        public int slotNumber = 1;

        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLog;

        public void Save() 
        {
            activeState = HistoryState.Capture();
            historyLog = HistoryManager.Instance.history.ToArray();
            activeConversations = GetConversationData();

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON);
        }

        public void Load() 
        {
            if (activeState != null)
                activeState.Load();
            
            HistoryManager.Instance.history = historyLog.ToList();
            HistoryManager.Instance.logManager.Clear();
            HistoryManager.Instance.logManager.Rebuild();

            SetConversationData();
            
            //Esconder el cursor.
            //DialogController.Instance.prompt.Hide();
        }

        public string[] GetConversationData()
        {
            List<string> retData = new List<string>();

            var conversations = DialogController.Instance.convManager.GetConversationQueue();

            int i = 0;
            for (i = 0; i < conversations.Length; i++)
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
            int i = 0;
            for(i = 0; i < activeConversations.Length; i++) 
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
    }
}

