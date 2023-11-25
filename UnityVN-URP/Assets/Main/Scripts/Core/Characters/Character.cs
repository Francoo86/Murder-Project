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

    //Cada personaje tendrá su propio nombre.
    public Character(string name) { 
        this.name = name;
        displayName = name;
        Debug.Log($"Creating character in base: {name}");
    }

    public DialogController DController => DialogController.Instance;

    //Hacer que el personaje hable.
    public Coroutine Say(string dialog) => Say(new List<string> { dialog});
    public Coroutine Say(List <string> dialogLines)
    {
        DController.ShowSpeakerName(displayName);
        return DController.Say(dialogLines);
    }


    public enum CharacterType
    {
        //Los mas usados en VN.
        Text,
        Sprite,
        SpriteSheet,
        //Esto no, lo dejo aqui por si llegase a ocurrir algo inesperado.
        //NO SE OCUPARA POR TEMAS DE TIEMPO, APARTE PARA EL ULTIMO HAY QUE MODELAR XD.
        Live2D,
        Model3D
    }
}
