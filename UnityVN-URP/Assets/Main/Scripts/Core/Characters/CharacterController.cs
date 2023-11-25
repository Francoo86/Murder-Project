using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //Estos objetos de tipo controlador solo existen una sola vez en el "sistema".
    public static CharacterController Instance { get; private set; }

}
