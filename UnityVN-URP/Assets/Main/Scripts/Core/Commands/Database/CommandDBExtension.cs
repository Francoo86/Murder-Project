using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Para poder a�adir comandos personalizados seg�n Unity.
public abstract class CommandDBExtension
{
    public static void Extend(CommandDB database) { 
    }

    public static CommandParameters ConvertToParams(string[] data) => new CommandParameters(data);
}
