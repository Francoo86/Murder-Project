using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;

/// <summary>
/// Builds or appends the text to the screen with an effect.
/// </summary>
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

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    //Text speed handler.
    public float speed { get { return DEFAULT_TEXT_SPEED * speedMultiplier; } set { speedMultiplier = value; } }
    private const float DEFAULT_TEXT_SPEED = 1;
    private float speedMultiplier = 1;

    public bool shouldSpeedUp = false;

    public int CharsPerCycle { get { return speed <= 2f ? characterMul : speed <= 2.5f ? characterMul * 2 : characterMul * 3; } }
    private int characterMul = 1;

    /// <summary>
    /// Gets a nice text speed for how the text will be displayed.
    /// </summary>
    /// <returns>Text speed.</returns>
    private float GetProperTextSpeed() { 
        return CharsPerCycle * (shouldSpeedUp? FASTER_TEXT_SPEED : 1);
    }

    /// <summary>
    /// Initializes the TextArchitect with the TextMeshProUGUI (used by panels with text) object.
    /// </summary>
    /// <param name="tmpro_ui"></param>
    public TextArchitect(TextMeshProUGUI tmpro_ui) {
        this.tmpro_ui = tmpro_ui;
    }

    /// <summary>
    /// Initializes the TextArchitect with the TextMeshPro object.
    /// </summary>
    /// <param name="tmpro_world"></param>
    public TextArchitect(TextMeshPro tmpro_world) {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// Wrapping function to start coroutines related to text building-appending.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="oldText">Old text that was assigned before, in the build is empty by default.</param>
    /// <returns></returns>
    private Coroutine StartTextCoroutine(string text, string oldText = "") {
        preText = oldText;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// Initializes the text of the characters to show it after.
    /// It will replace text.
    /// </summary>
    /// <param name="text">The text or dialog in this case.</param>
    /// <returns>The coroutine that builds the text.</returns>
    public Coroutine Build(string text) {
        return StartTextCoroutine(text);
    }

    /// <summary>
    /// Appends text to an existant dialog.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <returns>The coroutine that appends the new text.</returns>
    public Coroutine Append(string text)
    {
        return StartTextCoroutine(text, tmpro.text);
    }

    public Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    /// <summary>
    /// Tells the writing coroutine to stop.
    /// </summary>
    public void Stop() {
        if (!isBuilding) return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building() {
        Prepare();

        yield return Build_TypeWriter();

        OnComplete();
    }

    private void OnComplete() {
        buildProcess = null;
    }

    /// <summary>
    /// Prepares the text by the specified ones.
    /// </summary>
    private void Prepare() 
    {
        Prepare_Typewriter();
    }

    public void ForceComplete() {
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        Stop();
        OnComplete();
    }

    /// <summary>
    /// Prepares the Typewriting effect for VN.
    /// </summary>
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
    /// Builds the typewriting effect for the text.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Build_TypeWriter() {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount) {
            tmpro.maxVisibleCharacters += (int)GetProperTextSpeed();
            yield return new WaitForSeconds(BASE_TEXT_SPEED / speed);
        }
    }
}
