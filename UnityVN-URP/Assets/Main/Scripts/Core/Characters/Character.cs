using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public string name;
    public string displayName;
    //Hacer un cuadro para cada imagen de personaje.
    public RectTransform root;
    public CharacterConfigData config;

    //Cada personaje tendr� su propio nombre.
    public Character(string name, CharacterConfigData config) { 
        this.name = name;
        this.config = config;
        displayName = name;
        Debug.Log($"Creating character in base: {name}");
    }

    public DialogController DController => DialogController.Instance;

    //Hacer que el personaje hable.
    public Coroutine Say(string dialog) => Say(new List<string> { dialog});
    public Coroutine Say(List <string> dialogLines)
    {
        DController.ShowSpeakerName(displayName);
        DController.ApplySpeakerDataToBox(config);
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
