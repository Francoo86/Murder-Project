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
        root.SetActive(false);
    }
}
