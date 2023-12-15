using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //Le pegue una vuelta, y era porque no estaba el codigo de las fuentes en la carpeta.
            //Es esta Resources/Fonts, btw no creo que sea muy relevante el tema de las fuentes.
            data.dialogueFont = FilePaths.ResourcesFonts + dialogueText.font.name;

            return data;
        }
    }
}
