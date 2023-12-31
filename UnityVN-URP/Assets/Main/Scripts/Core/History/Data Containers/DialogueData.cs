using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace History
{
    /// <summary>
    /// Class that holds information of the current dialog on the screen, in this case, the character name, the text and saves both font and color.
    /// </summary>
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

        /// <summary>
        /// Captures the dialog data 
        /// </summary>
        /// <returns></returns>
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
            data.dialogueColor = dialogueText.color;
            data.dialogueScale = dialogueText.fontSize;

            data.currentSpeaker = nameText.text;
            data.speakerFont = FilePaths.ResourcesFonts + nameText.font.name;
            data.speakerNameColor = nameText.color;
            data.speakerScale = nameText.fontSize;

            return data;
        }

        /// <summary>
        /// Applies the data of the dialog.
        /// </summary>
        /// <param name="data">The data to be applied.</param>
        public static void Apply(DialogueData data)
        {
            var ds = DialogController.Instance;
            var dialogueText = ds.dialogContainer.dialogText;
            var nameText = ds.dialogContainer.nameContainer.nameText;

            dialogueText.text = data.currentDialogue;
            dialogueText.color = data.dialogueColor;
            dialogueText.fontSize = data.dialogueScale;

            nameText.text = data.currentSpeaker;
            if (nameText.text != string.Empty)
            {
                ds.dialogContainer.nameContainer.Show();
            }
            else
            {
                ds.dialogContainer.nameContainer.Hide();
            }

            nameText.color = data.speakerNameColor;
            nameText.fontSize = data.speakerScale;

            if (data.dialogueFont != dialogueText.font.name) 
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.dialogueFont);
                if (fontAsset != null)
                    dialogueText.font = fontAsset;
            }

            if (data.speakerFont != nameText.font.name)
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.speakerFont);
                if (fontAsset != null)
                    nameText.font = fontAsset;
            }
        }
    }
}
