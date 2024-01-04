using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace History
{
    /// <summary>
    /// Manages history states of the game.
    /// </summary>
    [RequireComponent(typeof(HistoryLogManager))]
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {

        public const int HISTORY_CACHE_LIMIT = 111;

        public static HistoryManager Instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private HistoryNavigation navigation;
        public HistoryLogManager logManager { get; private set; }

        /// <summary>
        /// Setups the instance, and attachs the LogManager and the navigation.
        /// </summary>
        private void Awake() 
        {
            Instance = this;
            navigation = GetComponent<HistoryNavigation>();
            logManager = GetComponent<HistoryLogManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            DialogController.Instance.onClear += LogCurrentState;
        }

        /// <summary>
        /// Reloads log manager by clearing it and rebuilding it.
        /// </summary>
        public void ResetLogManager()
        {
            logManager.Clear();
            logManager.Rebuild();
        }

        /// <summary>
        /// Gets the current history state and appends it to the log, only if the limit is not surpassed.
        /// </summary>
        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);
            logManager.AddLog(state);

            if (history.Count > HISTORY_CACHE_LIMIT)
                history.RemoveAt(0);
        }

        /// <summary>
        /// Loads the current history state.
        /// </summary>
        /// <param name="state">The passed state.</param>
        public void LoadState(HistoryState state)
        {
            state.Load();
        }

        /// <summary>
        /// Makes the history advance until current state.
        /// </summary>
        public void GoFoward() => navigation.GoFoward();

        /// <summary>
        /// Makes the history go back until it meets the first dialog ocurrence.
        /// </summary>
        public void GoBack() => navigation.GoBack();

    }
}
