using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace History
{
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {

        public static HistoryManager Instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private HistoryNavigation navigation;

        private void Awake() 
        {
            Instance = this;
            navigation = GetComponent<HistoryNavigation>();
        }

        // Start is called before the first frame update
        void Start()
        {
            DialogController.instance.OnClear() += LogCurrentState;
        }

        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);
        }

        public void LoadState(HistoryState state)
        {
            state.Load();
        }

        public void GoFoward() => navigation.GoFoward();

        public void GoBack() => navigation.GoBack();

    }
}
