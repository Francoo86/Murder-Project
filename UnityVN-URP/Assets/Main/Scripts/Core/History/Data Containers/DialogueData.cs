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

            var ds = DialogController.Instance;
            var dialogueText = ds.dialogContainer.dialogText;
            var nameText = ds.dialogContainer.nameContainer.nameText;

            data.currentDialogue = dialogueText.text;
            data.dialogueFont = FilePaths.dialogueText.font.name;

            return data;
        }
    }
}
