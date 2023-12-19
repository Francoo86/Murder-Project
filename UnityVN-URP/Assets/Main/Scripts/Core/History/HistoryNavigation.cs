using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace History 
{ 
    public class HistoryNavigation : MonoBehaviour
    {
        public int progress = 0;

        [SerializeField] private TextMeshProUGUI statusText;

        HistoryManager manager => HistoryManager.Instance;
        List<HistoryState> history => manager.history;

        HistoryState cachedState = null;
        private bool isOnCachedState = false;

        public bool isViewingHistory = false;

        public bool canNavigate => !DialogController.Instance.convManager.isOnLogicalLine;

        public void GoFoward() 
        {
            if (!isViewingHistory || !canNavigate) 
                return;

            HistoryState state = null;

            if (progress < history.Count - 1)
            {
                progress++;
                state = history[progress];
            }
            else 
            {
                isOnCachedState = true;
                state = cachedState;
            }

            state.Load();

            if (isOnCachedState)
            {
                isViewingHistory = false;
                DialogController.Instance.onUserPrompt_Next -= GoFoward;
                statusText.text = "";
                DialogController.Instance.OnStopViewingHistory();
            }
            else 
            {
                UpdateStatusText();
            }
        }
        
        public void GoBack() 
        {
            if ( (progress == 0 && isViewingHistory) || !canNavigate)
                return;

            progress = isViewingHistory ? progress - 1 : history.Count - 1;

            if (!isViewingHistory)
            {
                isViewingHistory = true;
                isOnCachedState = false;
                cachedState = HistoryState.Capture();

                DialogController.Instance.onUserPrompt_Next += GoFoward;
                DialogController.Instance.OnStartViewingHistory();
            }
            HistoryState state = history[progress];
            state.Load();
            UpdateStatusText();
        }

        private void UpdateStatusText() 
        {
            statusText.text = $"{history.Count - progress}/{history.Count}";
        }

    }
}
