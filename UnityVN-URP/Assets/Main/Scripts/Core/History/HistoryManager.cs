using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace History
{
    [RequireComponent(typeof(HistoryLogManager))]
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {

        public const int HISTORY_CACHE_LIMIT = 111;

        public static HistoryManager Instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private HistoryNavigation navigation;
        public HistoryLogManager logManager { get; private set; }

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

        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);
            logManager.AddLog(state);

            if (history.Count > HISTORY_CACHE_LIMIT)
                history.RemoveAt(0);
        }

        public void LoadState(HistoryState state)
        {
            state.Load();
        }

        public void GoFoward() => navigation.GoFoward();

        public void GoBack() => navigation.GoBack();

    }
}
