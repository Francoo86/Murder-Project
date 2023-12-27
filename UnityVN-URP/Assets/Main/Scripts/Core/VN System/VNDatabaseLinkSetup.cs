using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{
    public class VNDatabaseLinkSetup : MonoBehaviour
    {
        public void SetupExternalLinks() 
        {
            Debug.Log($"Variable stuff like playername: {VNGameSave.activeFile}");
            VariableStore.CreateVariable("VN.mainCharName", "", () => VNGameSave.activeFile.playerName, value => VNGameSave.activeFile.playerName = value);
        }
    }
}
