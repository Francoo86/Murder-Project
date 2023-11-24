using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    public static CommandController Instance { get; private set; }
    private CommandDB cmdDatabase;

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
    public void Execute(string command) { 
        Delegate cmd = cmdDatabase.GetCommand(command);
        cmd?.DynamicInvoke(null);
    }
}
