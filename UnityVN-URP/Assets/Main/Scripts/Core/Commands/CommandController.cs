using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    public static CommandController Instance { get; private set; }
    private CommandDB cmdDatabase;
    private static Coroutine process = null;
    public static bool IsRunning => process != null;

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
    public Coroutine Execute(string command, params string[] args) { 
        Delegate cmd = cmdDatabase.GetCommand(command);
        //cmd?.DynamicInvoke(null);
        if (cmd == null) return null;

        return StartProcess(command, cmd, args);
    }

    private Coroutine StartProcess(string commandName, Delegate command, string[] args) {
        StopCurrentProcess();
        process = StartCoroutine(RunningProc(command, args));
        return process;
    }

    private void StopCurrentProcess()
    {
        if (process != null)
        {
            StopCoroutine(process);
        }

        process = null;
    }

    //Esto es para poder correr el proceso dentro de otra corutina.
    private IEnumerator RunningProc(Delegate commandProc, string[] args)
    {
        yield return WaitForCommandToFinish(commandProc, args);
        process = null;
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
}
