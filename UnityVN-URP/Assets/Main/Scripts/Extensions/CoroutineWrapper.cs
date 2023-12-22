using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esta clase nos permitirá saber si la corutina termino.
//Aparte nos ayudará con el tema de que podremos controlar un bucle hasta que termine.
/// <summary>
/// Adds more functionality to the Coroutine, that let us manipulate the coroutine before it ends.
/// </summary>
public class CoroutineWrapper
{
    private MonoBehaviour owner;
    private Coroutine coroutine;

    public bool IsDone = false;
    /// <summary>
    /// Initializes the object.
    /// </summary>
    /// <param name="owner">A MonoBehaviour object that will perform the Coroutine actions.</param>
    /// <param name="coroutine">The Coroutine associated with the owner.</param>
    public CoroutineWrapper(MonoBehaviour owner, Coroutine coroutine)
    {
        this.owner = owner;
        this.coroutine = coroutine;
    }

    /// <summary>
    /// Stops the coroutine and ends it.
    /// </summary>
    public void Stop()
    {
        owner.StopCoroutine(coroutine);
        IsDone = true;
    }
}
