using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace History
{
    /// <summary>
    /// Class that holds the information about a single history log in the game.
    /// </summary>
    public class HistoryLog
    {
        public GameObject container;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;
        public float nameFontSize = 0;
        public float dialogueFontSize = 0;
    }
}
