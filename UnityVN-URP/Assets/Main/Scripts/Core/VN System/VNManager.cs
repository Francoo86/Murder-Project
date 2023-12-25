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
            VNDatabaseLinkSetup linksetup = GetComponent<VNDatabaseLinkSetup>();
            linksetup.SetupExternalLinks();

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
                Debug.Log("WHY THIS SHIT HERE?");
                List<string> lines = FileManager.ReadTextAsset(config.startingFile);
                Conversation start = new Conversation(lines);
                DialogController.Instance.Say(start);
            }
            else
            {
                Debug.Log("IS FIXED!");
                VNGameSave.activeFile.Activate();
            }
        }

    }
}