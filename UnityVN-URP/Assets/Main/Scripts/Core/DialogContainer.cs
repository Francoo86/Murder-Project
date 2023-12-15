using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
/// <summary>
/// Clase encargada de la interfaz del dialogo.
/// </summary>
public class DialogContainer
{
    public GameObject root;
    //Personaje.
    public NameContainer nameContainer;
    //Lo que va a decir.
    public TextMeshProUGUI dialogText;
    private CanvasGroupController CanvasController;

    //TODO: Implement interface of this thing.
    public void SetDialogColor(Color color) => dialogText.color = color;
    public void SetDialogFont(TMP_FontAsset font) => dialogText.font = font;

    private bool initialized = false;

    public void Initialize()
    {
        if(initialized) return;

        CanvasController = new CanvasGroupController(DialogController.Instance, root.GetComponent<CanvasGroup>());
    }

    public bool IsVisible => CanvasController.IsVisible;
    public Coroutine Show(float speed = 1, bool inmediate = false) => CanvasController.Show(speed, inmediate);
    public Coroutine Hide(float speed = 1, bool inmediate = false) => CanvasController.Hide(speed, inmediate);
}
