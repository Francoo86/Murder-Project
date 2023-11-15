using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    private const float BASE_TEXT_SPEED = 0.015f;
    private const int FASTER_TEXT_SPEED = 5;

    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    public string currentText => tmpro.text;
    public string targetText { get; private set; } = "";
    public string preText { get; private set; } = "";

    //Usado más para los fades.
    private int preTextLength = 0;

    public string fullTargetText => preText + targetText;
    public enum BuildMethod { instant, typewriter, fade};
    public BuildMethod buildMethod = BuildMethod.typewriter;

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    //Text speed handler.
    public float speed { get { return textSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float textSpeed = 1;
    private float speedMultiplier = 1;

    public bool shouldSpeedUp = false;

    public int charsPerCycle { get { return speed <= 2f ? characterMul : speed <= 2.5f ? characterMul * 2 : characterMul * 3; } }
    private int characterMul = 1;

    public float GetProperTextSpeed() { 
        return charsPerCycle* (shouldSpeedUp? FASTER_TEXT_SPEED : 1);
    }

public TextArchitect(TextMeshProUGUI tmpro_ui) {
        this.tmpro_ui = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world) {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// Manejador de Corutinas para el dialogo.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="oldText"></param>
    /// <returns></returns>
    private Coroutine StartTextCoroutine(string text, string oldText = "") {
        preText = oldText;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// Construye el texto inicial que un personaje dirá.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Build(string text) {
        return StartTextCoroutine(text);
        /*
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;*/
    }

    /// <summary>
    /// Agrega el texto ya existente a un dialogo.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Append(string text)
    {
        /*
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;*/
        return StartTextCoroutine(text, tmpro.text);
    }


    public Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    public void Stop() {
        if (!isBuilding) return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building() {
        Prepare();

        switch (buildMethod) {
            case BuildMethod.typewriter:
                yield return Build_TypeWriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }

        OnComplete();
    }

    private void OnComplete() {
        buildProcess = null;
    }

    /// <summary>
    /// Prepara el texto para que no tenga errores.
    /// </summary>
    private void Prepare() 
    {
        switch (buildMethod) {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }

    public void ForceComplete() {
        switch (buildMethod) { 
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate();
                break;
        }

        Stop();
        OnComplete();
    }

    //TODO: Create a new class that holds this functionality.
    private void Prepare_Instant() {
        //Reinciar color.
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        //Cualquier cambio hecho al texto se actualizará aquí.
        tmpro.ForceMeshUpdate();
        //Que los caracteres calcen en la pantalla.
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter() {
        //Resetear el texto.
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        //Revisa si el texto viejo sigue vigente.
        if (preText != "") { 
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    /// <summary>
    /// Prepara al texto para que pueda hacer fade. (Efecto visual).
    /// </summary>
    private void Prepare_Fade()
    {
        tmpro.text = preText;
        if (preText != "") { 
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else
            preTextLength = 0;

        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color visibleCol = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color hiddenCol = new Color(textColor.r, textColor.g, textColor.b, 0);

        //Colores que se manejan desde 0-255.
        Color32[] vertexCols = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for (int i = 0; i < textInfo.characterCount; i++) { 
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if(!charInfo.isVisible) { 
                continue;
            }

            if (i < preTextLength)
            {
                for (int j = 0; j < 4; j++)
                    vertexCols[charInfo.index + j] = visibleCol;
            }
            else {
                for (int j = 0; j < 4; j++)
                    vertexCols[charInfo.index + j] = hiddenCol;

            }
        }

        //Actualizar las referencias del color del texto.
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private IEnumerator Build_TypeWriter() {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount) {
            tmpro.maxVisibleCharacters += (int)GetProperTextSpeed();
            yield return new WaitForSeconds(BASE_TEXT_SPEED / speed);
        }
        //yield return null;
    }

    private IEnumerator Build_Fade() {
        //TODO: HIPER-REFACTOR.
        int minRange = preTextLength;
        int maxRange = minRange + 1;
        int alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];


        while (true) {
            for (int i = minRange; i < maxRange; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if(!charInfo.isVisible) continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                //Hacer una especie de interpolación lineal, que le daría una transición más bonita.
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, GetProperTextSpeed() * 4f);

                for (int j = 0; j < 4; j++) {
                    vertexColors[charInfo.vertexIndex + j].a = (byte)alphas[i];
                }

                if (alphas[i] >= 255) {
                    minRange++;
                }

                tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                //Revisa si el caracter es invisible o si se excedio del alpha (no hay transparencia).
                bool isLastCharInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;

                if (alphas[maxRange - 1] > alphaThreshold || isLastCharInvisible) {
                    if (maxRange < textInfo.characterCount)
                    {
                        maxRange++;
                    }
                    else if (alphas[maxRange - 1] >= 255 || isLastCharInvisible)
                        break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
        //yield return null;
    }
}
