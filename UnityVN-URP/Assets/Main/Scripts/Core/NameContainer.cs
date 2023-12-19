using TMPro;
using UnityEngine;

/// <summary>
/// Class that holds the name of the character in the dialog box.
/// </summary>
/// 
[System.Serializable]
public class NameContainer
{
    [SerializeField] public GameObject root;
    [field:SerializeField] public TextMeshProUGUI nameText { get; private set; }
    
    /// <summary>
    /// Shows the character name box on the screen.
    /// </summary>
    /// <param name="newName">The character name to be displayed, if its the narrator it will be not displayed.</param>
    public void Show(string newName = "") {
        root.SetActive(true);

        if (newName != string.Empty) { 
            nameText.text = newName;
        }
    }

    /// <summary>
    /// Hides the character name.
    /// </summary>
    public void Hide() {
        //Debug.Log($"We are hiding the {nameText.name}");
        root.SetActive(false);
    }

    /// <summary>
    /// Sets the name color, by default is white.
    /// </summary>
    /// <param name="color">New color.</param>
    public void SetNameColor(Color color) => nameText.color = color;
    /// <summary>
    /// Sets the font of the character name, it can be configured.
    /// </summary>
    /// <param name="font">The TMPPro font.</param>
    public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
}