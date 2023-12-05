using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandGeneralExtension : CommandDBExtension
{
    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("wait", new Func<string, IEnumerator>(Wait));
    }

    private static IEnumerator Wait(string data) { 
        if(float.TryParse(data, out float result))
        {
            yield return new WaitForSeconds(result);
        }
    }
}
