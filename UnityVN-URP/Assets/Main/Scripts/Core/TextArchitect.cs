using System.Collections;
using UnityEngine;
using TMPro;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    public string currentText => tmpro.text;
    public string targetText { get; private set; } = "";
    public string preText { get; private set; } = "";
    private int preTextLength = 0;

    public string fullTargetText => preText + targetText;
    public enum BuildMethod { instant, typewriter, fade};
    public BuildMethod buildMethod = BuildMethod.typewriter;

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    //Text speed handler.
    public float speed { get { return textSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float textSpeed = 1;
    private float speedMultiplier = 1;

    public bool veryFastText = false;

    public int charsPerCycle { get { return speed <= 2f ? characterMul : speed <= 2.5f ? characterMul * 2 : characterMul * 3; } }
    private int characterMul = 1;

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

    //TODO: Create a new class that holds this functionality.
    private void Prepare_Instant() {
        //Reinciar color.
        tmpro.color = tmpro.color;
        tmpro.text = currentText;
        //Cualquier cambio hecho al texto se actualizará aquí.
        tmpro.ForceMeshUpdate();
        //Que los caracteres calcen en la pantalla.
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter() { 
    }

    private void Prepare_Fade()
    {

    }

    private IEnumerator Build_TypeWriter() {
        yield return null;
    }

    private IEnumerator Build_Fade() {
        yield return null;
    }
}
