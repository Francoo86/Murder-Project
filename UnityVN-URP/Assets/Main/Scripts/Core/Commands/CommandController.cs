using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the command execution (defined in the CommandDBExtension inherited classes) that can be run inside of the dialog file.
/// </summary>
public class CommandController : MonoBehaviour
{
    public static CommandController Instance { get; private set; }
    private CommandDB cmdDatabase;
    //Procesos actuales de distintos comandos.
    private List<CommandProcess> processList = new List<CommandProcess>();
    private CommandProcess topProcess => processList.Last();
    //private static Coroutine process = null;
    //public static bool IsRunning => process != null;

    /// <summary>
    /// Tries to load all defined methods that were inherited from CommandDBExtension. And runs the Extend method from them.
    /// </summary>
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
    
    /// <summary>
    /// Loads and executes a command defined from CommandDBExtension inherited methods.
    /// </summary>
    /// <param name="command">The command name.</param>
    /// <param name="args">The params to that command (optional).</param>
    /// <returns>The CoroutineWrapper to control the flow of execution of that command.</returns>
    public CoroutineWrapper Execute(string command, params string[] args) { 
        Delegate cmd = cmdDatabase.GetCommand(command);
        if (cmd == null) return null;

        return StartProcess(command, cmd, args);
    }

    /// <summary>
    /// Internal method that executes the loaded command into the Coroutine.
    /// Loads the command and associates it with a process.
    /// </summary>
    /// <param name="commandName"></param>
    /// <param name="command"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args) {
        Guid procID = Guid.NewGuid();

        CommandProcess cmdProc = new CommandProcess(procID, commandName, command, args, null, null);
        processList.Add(cmdProc);

        Coroutine co = StartCoroutine(RunningProc(cmdProc));
        cmdProc.currentProcess = new CoroutineWrapper(this, co);

        return cmdProc.currentProcess;
    }

    /// <summary>
    /// Stops the current running command.
    /// </summary>
    public void StopCurrentProcess()
    {
        if(topProcess != null)
            KillProcess(topProcess);
    }

    //Esto es para poder correr el proceso dentro de otra corutina.
    /// <summary>
    /// Runs the command into next iteration or yield.
    /// </summary>
    /// <param name="cmdProc">The command process.</param>
    /// <returns>The IEnumerator to be yielded.</returns>
    private IEnumerator RunningProc(CommandProcess cmdProc)
    {
        yield return WaitForCommandToFinish(cmdProc.command, cmdProc.args);
        KillProcess(cmdProc);
    }

    /// <summary>
    /// Kills a command and stops its execution, also removes it from the list of processes.
    /// </summary>
    /// <param name="cmdProc">The command process to be killed.</param>
    public void KillProcess(CommandProcess cmdProc)
    {
        processList.Remove(cmdProc);

        if (cmdProc.currentProcess != null && !cmdProc.currentProcess.IsDone)
            cmdProc.currentProcess.Stop();

        cmdProc.onTerminateAction?.Invoke();
    }

    //THIS SHOULDN'T STOP THE INWORLD PROCESS!!!!!!
    /// <summary>
    /// Stops all the process of the list.
    /// </summary>
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
    /// <summary>
    /// Runs the passed command function with its associated arguments.
    /// </summary>
    /// <param name="commandProc">The command function.</param>
    /// <param name="args">The parameters defined in CommandDBExtension inherited methods.</param>
    /// <returns></returns>
    private IEnumerator WaitForCommandToFinish(Delegate commandProc, string[] args) {
        if (commandProc is Action)
            commandProc.DynamicInvoke(null);
        //Es un comando que tiene strings como parametros.
        else if (commandProc is Action<string>)
            commandProc.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);
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

    /// <summary>
    /// Adds a function (listener) to be called when the first process ends.
    /// </summary>
    /// <param name="action">The callback function.</param>
    public void AddTerminationActionToActualProcess(UnityAction action)
    {
        CommandProcess proc = topProcess;
        if (proc == null) return;

        proc.onTerminateAction = new UnityEvent();
        proc.onTerminateAction.AddListener(action);
    }
}
