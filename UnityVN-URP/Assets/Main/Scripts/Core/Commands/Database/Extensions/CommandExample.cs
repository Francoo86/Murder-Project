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
        //database.AddCommand("test_moving", new Func<string, IEnumerator>(MoveCharacter));
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

    private static IEnumerator MoveCharacter(string dir)
    {
        bool left = dir.ToLower() == "left";
        Transform character = GameObject.Find("Body").transform;

        float moveSpeed = 45;
        float targetX = left ? -8 : 8;

        float oldOffset = character.position.x;

        while (Mathf.Abs(targetX - oldOffset) > 0.1f)
        {
            targetX = Mathf.MoveTowards(oldOffset, targetX, moveSpeed * Time.deltaTime);
            character.position = new Vector3(targetX, character.position.y, character.position.z);
            yield return null;
        }
    }
}
