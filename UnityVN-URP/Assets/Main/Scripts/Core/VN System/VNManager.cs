using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{
    /// <summary>
    /// Manages the preloading of the game.
    /// </summary>
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }
        [SerializeField] private VisualNovelSO config;
        public Camera mainCamera;

        /// <summary>
        /// Initializes the VN variables and tries to setup the activeFile of the session if it doesn't exists.
        /// </summary>
        private void Awake()
        {
            Instance = this;

            VariableStore.CreateVariable("VN.mainCharName", "", () => VNGameSave.activeFile.playerName, value => VNGameSave.activeFile.playerName = value);

            if (VNGameSave.activeFile == null)
                VNGameSave.activeFile = new VNGameSave();
        }

        /// <summary>
        /// After setup the VNGameSave loads the game.
        /// </summary>
        private void Start()
        {
            LoadGame();
        }

        /// <summary>
        /// Tries to load a game by either activating it (shows instantly the data on the screen) or if it doesn't exists
        /// uses the dialog file name passed in Unity.
        /// </summary>
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