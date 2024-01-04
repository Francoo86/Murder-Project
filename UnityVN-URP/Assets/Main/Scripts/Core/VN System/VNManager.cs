using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{

    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }
        [SerializeField] private VisualNovelSO config;
        public Camera mainCamera;

        private void Awake()
        {
            Instance = this;

            VariableStore.CreateVariable("VN.mainCharName", "", () => VNGameSave.activeFile.playerName, value => VNGameSave.activeFile.playerName = value);

            if (VNGameSave.activeFile == null)
                VNGameSave.activeFile = new VNGameSave();
        }

        private void Start()
        {
            LoadGame();
        }

        private void LoadGame()
        {
            if (VNGameSave.activeFile.newGame)
            {
                List<string> lines = FileManager.ReadTextAsset(config.startingFile);
                Conversation start = new Conversation(lines);
                DialogController.Instance.Say(start);
            }
            else
            {
                VNGameSave.activeFile.Activate();
            }
        }

    }
}