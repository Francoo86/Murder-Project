using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace History
{
    [System.Serializable]
    public class DialogueData
    {
        public string currentDialogue = "";
        public string currentSpeaker = "";

        public string dialogueFont;
        public Color dialogueColor;
        public float dialogueScale;

        public string speakerFont;
        public Color speakerNameColor;
        public float speakerScale;

        public static DialogueData Capture() 
        { 
            DialogueData data = new DialogueData();

            var ds = DialogSystem.instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            //var nameText = ds.dialogueContainer.nameContainer.

            return data;
        }
    }
}
