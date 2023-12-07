using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandGeneralExtension : CommandDBExtension
{
    private static readonly string[] PARAM_SPEED = new string[] { "-s", "-speed" };
    private static readonly string[] PARAM_INMEDIATE = new string[] { "-i", "-inmediate" };
    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("wait", new Func<string, IEnumerator>(Wait));
        commandDB.AddCommand("showdialog", new Func<string[], IEnumerator>(ShowDialog));
        commandDB.AddCommand("hidedialog", new Func<string[], IEnumerator>(HideDialog));

        commandDB.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogControl));
        commandDB.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogControl));
    }

    private static IEnumerator Wait(string data) { 
        if(float.TryParse(data, out float result))
        {
            yield return new WaitForSeconds(result);
        }
    }

    private static (float, bool) ExtractFadeLogic(string[] data)
    {
        float speed;
        bool inmediate;

        var parameters = ConvertToParams(data);

        parameters.TryGetValue(PARAM_SPEED, out speed, 1f);
        parameters.TryGetValue(PARAM_INMEDIATE, out inmediate, false);

        return (speed, inmediate);
    }

    private static IEnumerator ShowDialog(string[] data)
    {
        float speed;
        bool inmediate;
        (speed, inmediate) = ExtractFadeLogic(data);
        yield return DialogController.Instance.dialogContainer.Show(speed, inmediate);
    }

    private static IEnumerator HideDialog(string[] data)
    {
        float speed;
        bool inmediate;
        (speed, inmediate) = ExtractFadeLogic(data);
        yield return DialogController.Instance.dialogContainer.Hide(speed, inmediate);
    }

    private static IEnumerator ShowDialogControl(string[] data)
    {
        float speed;
        bool inmediate;
        (speed, inmediate) = ExtractFadeLogic(data);
        yield return DialogController.Instance.Show(speed, inmediate);
    }

    private static IEnumerator HideDialogControl(string[] data)
    {
        float speed;
        bool inmediate;
        (speed, inmediate) = ExtractFadeLogic(data);
        yield return DialogController.Instance.Hide(speed, inmediate);
    }
}
