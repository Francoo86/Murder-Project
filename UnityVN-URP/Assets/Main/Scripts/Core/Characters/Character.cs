using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public string name;
    //Hacer un cuadro para cada imagen de personaje.
    public RectTransform root;

    //Cada personaje tendrá su propio nombre.
    public Character(string name) { 
        this.name = name;
        Debug.Log($"Creating character in base: {name}");
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
