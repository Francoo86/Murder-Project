using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{

    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }
        public Camera mainCamera;

        private void Awake()
        {
            Instance = this;
            VNDatabaseLinkSetup linksetup = GetComponent<VNDatabaseLinkSetup>();
            linksetup.SetupExternalLinks();

            VNGameSave.activeFile = new VNGameSave();
        }

        public void LoadFile(string filePath)
        {
            List<string> lines = new List<string>();
            TextAsset file = Resources.Load<TextAsset>(filePath);

            try
            {
                lines = FileManager.ReadTextAsset(file);
            }
            catch
            {
                Debug.LogError($"Dialogue file at path 'Resources/{filePath}' does not exist!");
                return;
            }

            DialogController.Instance.Say(lines, filePath);
        }
    }
}