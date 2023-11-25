using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Character
{
    public string name;
    public string displayName;
    //Hacer un cuadro para cada imagen de personaje.
    public RectTransform root;
    public CharacterConfigData config;

    //Cada personaje tendrá su propio nombre.
    public Character(string name, CharacterConfigData config) {
        this.name = name;
        this.config = config;
        displayName = name;
        Debug.Log($"Creating character in base: {name}");
    }

    public DialogController DController => DialogController.Instance;

    public void SetDialogColor(Color col) => config.diagCol = col;
    public void SetNameColor(Color col) => config.nameCol = col;

    public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
    public void SetDialogFont(TMP_FontAsset font) => config.diagFont = font;

    public void UpdateConfigOnScreen() => DController.ApplySpeakerDataToBox(config);
    public void ResetCharacterConfig() => config = CharacterController.Instance.GetCharacterConfig(name);

    //Hacer que el personaje hable.
    public Coroutine Say(string dialog) => Say(new List<string> { dialog});
    public Coroutine Say(List <string> dialogLines)
    {
        DController.ShowSpeakerName(displayName);
        UpdateConfigOnScreen();
        return DController.Say(dialogLines);
    }


    public enum CharacterType
    {
        //Los mas usados en VN.
        Text,
        Sprite,
        SpriteSheet,
    }
}
