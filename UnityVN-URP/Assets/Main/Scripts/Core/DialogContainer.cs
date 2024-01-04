using UnityEngine;
using TMPro;

[System.Serializable]
/// <summary>
/// Class that holds the dialog on screen, it has the character name and the text box.
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
    /// <summary>
    /// Changes the dialog text color.
    /// </summary>
    /// <param name="color">Color object to be set.</param>
    public void SetDialogColor(Color color) => dialogText.color = color;

    /// <summary>
    /// Changes the dialog box font.
    /// </summary>
    /// <param name="font">The font to be set.</param>
    public void SetDialogFont(TMP_FontAsset font) => dialogText.font = font;

    //private bool initialized = false;

    /// <summary>
    /// Initializes the DialogContainer by associating the DialogController a CG Controller.
    /// </summary>
    public void Initialize()
    {
        //if(initialized) return;

        CanvasController = new CanvasGroupController(DialogController.Instance, root.GetComponent<CanvasGroup>());
    }

    public bool IsVisible => CanvasController.IsVisible;
    /// <summary>
    /// Shows the container on the screen. Wrapper function for CG Controller.
    /// </summary>
    /// <param name="speed">How fast it will show in the screen.</param>
    /// <param name="inmediate">Make it show instantly.</param>
    /// <returns>Coroutine process of showing.</returns>
    public Coroutine Show(float speed = 1, bool inmediate = false) => CanvasController.Show(speed, inmediate);

    /// <summary>
    /// Hides the container on the screen. Wrapper function for CG Controller.
    /// </summary>
    /// <param name="speed">How fast it will hide to be out of the screen.</param>
    /// <param name="inmediate">Make it hide instantly.</param>
    /// <returns>Coroutine process of hiding.</returns>
    public Coroutine Hide(float speed = 1, bool inmediate = false) => CanvasController.Hide(speed, inmediate);
}
