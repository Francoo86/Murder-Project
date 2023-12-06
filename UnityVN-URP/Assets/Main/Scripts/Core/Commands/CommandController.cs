using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class CommandController : MonoBehaviour
{
    public static CommandController Instance { get; private set; }
    private CommandDB cmdDatabase;
    //Procesos actuales de distintos comandos.
    private List<CommandProcess> processList = new List<CommandProcess>();
    private CommandProcess topProcess => processList.LastOrDefault();
    //private static Coroutine process = null;
    //public static bool IsRunning => process != null;

    private void Awake()
    {
        
        if(Instance == null)
        {
            Instance = this;
            cmdDatabase = new CommandDB();

            //Scans the current workspace.
            Assembly currentDir = Assembly.GetExecutingAssembly();
            Type[] extensionTypes = currentDir.GetTypes().Where(x => x.IsSubclassOf(typeof(CommandDBExtension))).ToArray();

            foreach(Type extension in extensionTypes)
            {
                MethodInfo extendMethod = extension.GetMethod("Extend");
                extendMethod.Invoke(null, new object[] { cmdDatabase }); 
            }
        }
        else
            DestroyImmediate(Instance);
    }
    // Start is called before the first frame update
    public CoroutineWrapper Execute(string command, params string[] args) { 
        Delegate cmd = cmdDatabase.GetCommand(command);
        if (cmd == null) return null;

        return StartProcess(command, cmd, args);
    }

    private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args) {
        Guid procID = Guid.NewGuid();

        CommandProcess cmdProc = new CommandProcess(procID, commandName, command, args, null, null);
        processList.Add(cmdProc);

        Coroutine co = StartCoroutine(RunningProc(cmdProc));
        cmdProc.currentProcess = new CoroutineWrapper(this, co);

        return cmdProc.currentProcess;
        /*
        StopCurrentProcess();
        process = StartCoroutine(RunningProc(command, args));
        return process;*/
    }

    public void StopCurrentProcess()
    {
        if(topProcess != null)
            KillProcess(topProcess);
    }

    //Esto es para poder correr el proceso dentro de otra corutina.
    private IEnumerator RunningProc(CommandProcess cmdProc)
    {
        yield return WaitForCommandToFinish(cmdProc.command, cmdProc.args);
        KillProcess(cmdProc);
    }

    public void KillProcess(CommandProcess cmdProc)
    {
        processList.Remove(cmdProc);

        if (cmdProc.currentProcess != null && !cmdProc.currentProcess.IsDone)
            cmdProc.currentProcess.Stop();

        cmdProc.onTerminateAction?.Invoke();
    }

    //THIS SHOULDN'T STOP THE INWORLD PROCESS!!!!!!
    public void StopAllProcesses()
    {
        foreach(var proc in processList)
        {
            CoroutineWrapper currProc = proc.currentProcess;
            if(currProc != null && !currProc.IsDone)
            {
                currProc.Stop();
            }

            proc.onTerminateAction?.Invoke();
        }

        processList.Clear();
    }

    //TODO: Make it to factory pattern.
    //Callback my beloved.
    private IEnumerator WaitForCommandToFinish(Delegate commandProc, string[] args) {
        if (commandProc is Action)
            commandProc.DynamicInvoke(null);
        //Es un comando que tiene strings como parametros.
        else if (commandProc is Action<string>)
            commandProc.DynamicInvoke(args[0]);
        //Tiene argumentos variables.
        else if (commandProc is Action<string[]>)
            commandProc.DynamicInvoke((object)args);
        //Tiene funciones como corutinas.
        else if (commandProc is Func<IEnumerator>)
            yield return ((Func<IEnumerator>)commandProc)();
        else if (commandProc is Func<string, IEnumerator>)
            yield return ((Func<string, IEnumerator>)commandProc)(args[0]);
        else if (commandProc is Func<string[], IEnumerator>)
            yield return ((Func<string[], IEnumerator>)commandProc)(args);
    }

    public void AddTerminationActionToActualProcess(UnityAction action)
    {
        CommandProcess proc = topProcess;
        if (proc == null) return;

        proc.onTerminateAction = new UnityEvent();
        proc.onTerminateAction.AddListener(action);
    }
}
