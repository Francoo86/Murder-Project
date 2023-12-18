using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Command (methods in the dialog file)
/// </summary>
public class CommandProcess
{
    //Identificador del proceso.
    public Guid ID;
    //Esto es para saber que comando estamos utilizando en el momento.
    public string processName;
    //Que funcion se está utilizando.
    public Delegate command;
    //Argumentos.
    public string[] args;
    //Usar el wrapper.
    public CoroutineWrapper currentProcess;
    
    //Tener control de lo que se ejecuta en escena mientras se presionan teclas.
    public UnityEvent onTerminateAction;

    public CommandProcess(Guid ID, string processName, Delegate command, string[] args, CoroutineWrapper currentProcess, UnityEvent onTerminateAction = null)
    {
        this.ID = ID;
        this.processName = processName;
        this.command = command;
        this.args = args;
        this.currentProcess = currentProcess;
        this.onTerminateAction = onTerminateAction;
    }
}
