using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class that customizes the dialog inside Unity.
/// </summary>
[CreateAssetMenu(fileName = "Dialog System Configuration", menuName ="Dialog System/Dialog System Configuration File")]
public class DialogConfig : ScriptableObject
{
    public CharacterConfig charConfigAsset;
    public Color defaultTextColor = Color.white;
    public TMP_FontAsset defaultFont;
}
