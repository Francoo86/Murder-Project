using UnityEngine;
using TMPro;

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
}
