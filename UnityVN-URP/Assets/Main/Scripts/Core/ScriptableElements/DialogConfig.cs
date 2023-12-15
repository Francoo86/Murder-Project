using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Minuto 39.
[CreateAssetMenu(fileName = "Dialog System Configuration", menuName ="Dialog System/Dialog System Configuration File")]
public class DialogConfig : ScriptableObject
{
    public CharacterConfig charConfigAsset;
    public Color defaultTextColor = Color.white;
    public TMP_FontAsset defaultFont;
}
