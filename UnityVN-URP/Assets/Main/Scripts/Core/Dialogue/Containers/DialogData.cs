using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class that holds all the segments and the info of a dialog line.
/// Also supports simple commands like wait and stuff.
/// </summary>
public class DialogData
{
    public string RawData { get; private set; } = string.Empty;
    public List<DIALOG_SEGMENT> segments;
    /// <summary>
    /// Busca primero si hay Clear o Append, o también si hay Wait para esos dos.
    /// Siguiendo con la expresion regular, d sería el delay en segundos (hay 2 "d" ya que es un float.).
    /// </summary>
    private const string segmentIdPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";
    public bool HasDialog => segments.Count > 0;
    /// <summary>
    /// Initializes the object and extracts the segments of the dialog.
    /// </summary>
    /// <param name="rawDialog">The full dialog.</param>
    public DialogData(string rawDialog) { 
        RawData = rawDialog;
        segments = ExtractSegments(rawDialog);
    }

    /// <summary>
    /// Extracts the segments of the dialog and tries to do some actions (if they are specified) like wait.
    /// </summary>
    /// <param name="rawDialog">The full dialog.</param>
    /// <returns>All dialog segments.</returns>
    private List<DIALOG_SEGMENT> ExtractSegments(string rawDialog) {
        List <DIALOG_SEGMENT> currentSegments = new List <DIALOG_SEGMENT>();
        MatchCollection matches = Regex.Matches(rawDialog, segmentIdPattern);

        //Encotrar el primer o ultimo segmento en el archivo.
        DIALOG_SEGMENT segment = new DIALOG_SEGMENT();
        segment.dialog = (matches.Count == 0 ? rawDialog : rawDialog.Substring(0, matches[0].Index));
        segment.startSignal = DIALOG_SEGMENT.StartSignal.NONE;
        segment.signalDelay = 0;

        //Añadir a segmentos.
        currentSegments.Add(segment);


        int lastIndex;
        if (matches.Count == 0)
            return currentSegments;
        else
            lastIndex = matches[0].Index;

        for (int i = 0; i < matches.Count; i++) { 
            Match match = matches[i];
            segment = new DIALOG_SEGMENT();

            string signalMatch = match.Value;
            //De -2 ya que queremos de {A} queremos solamente A.
            signalMatch = signalMatch.Substring(1, match.Length - 2);
            //Pythonic way to split this.
            string[] signalSplit = signalMatch.Split(' ');

            //Castear en relacion a la letra encontrada por ejemplo "A" debería ser 2.
            segment.startSignal = (DIALOG_SEGMENT.StartSignal) Enum.Parse(typeof(DIALOG_SEGMENT.StartSignal), signalSplit[0].ToUpper());

            //Obtener el delay de la señal.
            if(signalSplit.Length > 1)
                float.TryParse(signalSplit[1], out segment.signalDelay);

            //Obtener el dialogo para el segmento.
            int nextIdx = i + 1 < matches.Count ? matches[i + 1].Index : rawDialog.Length;
            segment.dialog = rawDialog.Substring(lastIndex + match.Length, nextIdx - (lastIndex + match.Length));

            lastIndex = nextIdx;
            currentSegments.Add(segment);
        }

        return currentSegments;
    }

    /// <summary>
    /// Estructura que guarda información de un segmento de dialogo.
    /// Guarda también la signal de comienzo de un wait, clear o agregar texto.
    /// </summary>
    public struct DIALOG_SEGMENT {
        public string dialog;
        public StartSignal startSignal;
        public float signalDelay;

        public enum StartSignal
        {
            /// <summary>
            /// Explicacion:
            /// Cada enumeración corresponde a la forma de como el dialogo se va a mostrar de acuerdo a un segmento.
            /// NONE = Nada.
            /// C = Limpiar el texto.
            /// A = Agregar texto.
            /// WA = Esperar a agregar texto.
            /// WC = Esperar a limpiar.
            /// </summary>
            NONE, C, A, WA, WC
        }

        public readonly bool ShouldAppend => startSignal == StartSignal.A || startSignal == StartSignal.WA;
    }
}
