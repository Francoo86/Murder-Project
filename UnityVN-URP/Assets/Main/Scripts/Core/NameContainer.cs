using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Clase encargada de manejar la logica del nombre del personaje de la caja de dialogo.
/// </summary>
/// 
[System.Serializable]
public class NameContainer
{
    [SerializeField] public GameObject root;
    [SerializeField] public TextMeshProUGUI nameText;
    // Start is called before the first frame update
    public void Show(string newName = "") {
        root.SetActive(true);

        if (newName != string.Empty) { 
            nameText.text = newName;
        }
    }

    public void Hide() {
        //Debug.Log($"We are hiding the {nameText.name}");
        root.SetActive(false);
    }

    public void SetNameColor(Color color) => nameText.color = color;
    public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
}