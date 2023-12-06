using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esta clase nos permitir� saber si la corutina termino.
//Aparte nos ayudar� con el tema de que podremos controlar un bucle hasta que termine.
public class CoroutineWrapper : MonoBehaviour
{
    private MonoBehaviour owner;
    private Coroutine coroutine;

    public bool IsDone = false;
    public CoroutineWrapper(MonoBehaviour owner, Coroutine coroutine)
    {
        this.owner = owner;
        this.coroutine = coroutine;
    }

    public void Stop()
    {
        owner.StopCoroutine(coroutine);
        IsDone = true;
    }
}
