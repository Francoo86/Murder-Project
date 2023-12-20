using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISAULNOVEL
{

    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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