using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExample : CommandDBExtension
{
    new public static void Extend(CommandDB database)
    {
        database.AddCommand("Printear", new Action(PrintStuff));
        database.AddCommand("process_test", new Func<IEnumerator>(PrintNumbers));
    }

    private static void PrintStuff() {
        Debug.Log("Printing some stuff with this command.");
    }

    private static IEnumerator PrintNumbers() { 
        for(int i = 0; i < 10; i++)
        {
            Debug.Log($"Printing number inside coroutine: {i}");
            yield return new WaitForSeconds(1);
        }
    }
}
