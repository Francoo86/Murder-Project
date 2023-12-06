using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandProcess
{
    //Identificador del proceso.
    public Guid ID;
    //Esto es para saber que comando estamos utilizando en el momento.
    public string processName;
    //Que funcion se est� utilizando.
    public Delegate command;
    //Argumentos.
    public string[] args;
    //Usar el wrapper.
    public CoroutineWrapper currentProcess;
    
    //Tener control de lo que se ejecuta en escena mientras se presionan teclas.
    public UnityEvent onTerminateAction;

    public CommandProcess(Guid iD, string processName, Delegate command, string[] args, CoroutineWrapper currentProcess, UnityEvent onTerminateAction = null)
    {
        ID = iD;
        this.processName = processName;
        this.command = command;
        this.args = args;
        this.currentProcess = currentProcess;
        this.onTerminateAction = onTerminateAction;
    }
}
