using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExample : CommandDBExtension
{
    new public static void Extend(CommandDB database)
    {
        database.AddCommand("Printear", new Action(PrintStuff));
    }

    private static void PrintStuff() {
        Debug.Log("Printing some stuff with this command.");
    }
}
