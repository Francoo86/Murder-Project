using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace History
{
    /// <summary>
    /// Handles the History Log stuff when player opens it.
    /// </summary>
    public class HistoryLogManager : MonoBehaviour
    {
        private const float LOG_STARTING_HEIGHT = 2F;
        private const float LOG_HEIGHT_PER_LINE = 2F;
        private const float LOG_DEFAULT_HEIGHT = 1F;
        private const float TEXT_DEFAULT_SCALE = 1F;

        private const string NAMETEXT_NAME = "NameText";
        private const string DIALOGUETEXT_NAME = "DialogueText";

        private float logScaling = 1f;

        [SerializeField] private GameObject logPrefab;
        [SerializeField] private CanvasGroup PanelCG;

        HistoryManager manager => HistoryManager.Instance;
        private List<HistoryLog> logs = new List<HistoryLog>();

        public bool isOpen { get; private set; } = false;

        [SerializeField] private Slider logScaleSlider;

        private float textScaling => logScaling * 3f;

        /// <summary>
        /// Makes the HistoryLogs displayable on the screen.
        /// </summary>
        /// <param name="active">Should display.</param>
        private void SetActive(bool active)
        {
            PanelCG.alpha = active ? 1f : 0f;
            PanelCG.interactable = active;
            PanelCG.blocksRaycasts = active;
        }

        /// <summary>
        /// Open all the current logs until the last state.
        /// </summary>
        public void Open()
        {
            if (isOpen)
            {
                return;
            }

            Debug.Log($"Is being active? {isOpen}");
            SetActive(true);
            isOpen = true;
        }

        /// <summary>
        /// Closes the HistoryLog Panel.
        /// </summary>
        public void Close()
        {
            if (!isOpen)
            {
                return;
            }
            SetActive(false);
            isOpen = false;
        }

        /// <summary>
        /// Appends a log to the panel.
        /// </summary>
        /// <param name="state">The state that holds info about the character and the dialog.</param>
        public void AddLog(HistoryState state)
        {
            if (logs.Count >= HistoryManager.HISTORY_CACHE_LIMIT)
            {
                DestroyImmediate(logs[0].container);
                logs.RemoveAt(0);
            }

            CreateLog(state);
        }

        /// <summary>
        /// Creates a log based on the current history state, that has the character name and the dialog said in that time.
        /// </summary>
        /// <param name="state">The history state.</param>
        private void CreateLog(HistoryState state)
        {
            HistoryLog log = new HistoryLog();

            log.container = Instantiate(logPrefab, logPrefab.transform.parent);
            log.container.SetActive(true);

            log.nameText = log.container.transform.Find(NAMETEXT_NAME).GetComponent<TextMeshProUGUI>();
            log.dialogueText = log.container.transform.Find(DIALOGUETEXT_NAME).GetComponent<TextMeshProUGUI>();

            if (state.dialogue.currentSpeaker == string.Empty)
            {
                log.nameText.text = string.Empty;
            }
            else 
            {
                log.nameText.text = state.dialogue.currentSpeaker;
                log.nameText.font = HistoryCache.LoadFont(state.dialogue.speakerFont);
                log.nameText.color = state.dialogue.speakerNameColor;
                log.nameFontSize = TEXT_DEFAULT_SCALE * state.dialogue.speakerScale;
                log.nameText.fontSize = log.nameFontSize + textScaling;
            }

            log.dialogueText.text = state.dialogue.currentDialogue;
            log.dialogueText.font = HistoryCache.LoadFont(state.dialogue.dialogueFont);
            log.dialogueText.color = state.dialogue.dialogueColor;
            log.dialogueFontSize = TEXT_DEFAULT_SCALE * state.dialogue.dialogueScale;
            log.dialogueText.fontSize = log.dialogueFontSize + textScaling;

            //Debug.Log($"<color=#FFA500>Current log: {log.dialogueText.text}</color>");

            FitLogToText(log);
            logs.Add(log);
        }

        /// <summary>
        /// Makes responsive text to the log to avoid text overflowing.
        /// </summary>
        /// <param name="log">The history log.</param>
        private void FitLogToText(HistoryLog log)
        {
            RectTransform rect = log.dialogueText.GetComponent<RectTransform>();
            ContentSizeFitter textCSF = log.dialogueText.GetComponent<ContentSizeFitter>();

            textCSF.SetLayoutVertical();
            LayoutElement logLayout = log.container.GetComponent<LayoutElement>();
            float height = rect.rect.height;
            float perc = height / LOG_DEFAULT_HEIGHT;
            float extraScale = (LOG_HEIGHT_PER_LINE * perc) - LOG_HEIGHT_PER_LINE;
            float scale = LOG_STARTING_HEIGHT + extraScale;

            logLayout.preferredHeight = scale + textScaling;
            logLayout.preferredHeight += 2f * logScaling;
        }

        /// <summary>
        /// Makes the slider to scale the text of the logs.
        /// </summary>
        public void SetLogScaling()
        {
            logScaling = logScaleSlider.value;

            foreach ( HistoryLog log in logs ) 
            { 
                log.nameText.fontSize = log.nameFontSize + textScaling;
                log.dialogueText.fontSize = log.dialogueFontSize + textScaling;

                FitLogToText (log);
            }
        }

        /// <summary>
        /// Removes all logs and destroys them.
        /// </summary>
        public void Clear() 
        {
            int i = 0;
            for ( i=0; i<logs.Count; i++ ) 
                DestroyImmediate(logs[i].container);

            logs.Clear();
        }

        /// <summary>
        /// Creates all logs based on the HistoryManager history states.
        /// </summary>
        public void Rebuild() 
        {
            foreach (var state in manager.history)
            {
                CreateLog(state);
            }
        }
    }
}