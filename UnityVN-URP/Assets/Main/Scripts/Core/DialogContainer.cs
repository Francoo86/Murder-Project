using UnityEngine;
using TMPro;
using DIALOGUE; // Add this line

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

    //TODO: Implement interface of this thing.
    public void SetDialogColor(Color color) => dialogText.color = color;
    public void SetDialogFont(TMP_FontAsset font) => dialogText.font = font;
}
