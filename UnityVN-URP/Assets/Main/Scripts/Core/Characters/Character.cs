using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public string name;
    //Hacer un cuadro para cada imagen de personaje.
    public RectTransform root;

    public enum CharacterType
    {
        //Los mas usados en VN.
        None,
        Sprite,
        SpriteSheet,
        //Esto no, lo dejo aqui por si llegase a ocurrir algo inesperado.
        //NO SE OCUPARA POR TEMAS DE TIEMPO, APARTE PARA EL ULTIMO HAY QUE MODELAR XD.
        Live2D,
        Model3D
    }
}
