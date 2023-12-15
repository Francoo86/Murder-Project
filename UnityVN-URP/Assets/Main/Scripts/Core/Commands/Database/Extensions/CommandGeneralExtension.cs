using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandGeneralExtension : CommandDBExtension
{
    private static readonly string[] PARAM_SPEED = new string[] { "-s", "-speed" };
    private static readonly string[] PARAM_INMEDIATE = new string[] { "-i", "-inmediate" };

    //Cargar diferentes archivos como RenPy.
    private static readonly string[] PARAM_FILEPATH = new string[] { "-f", "-filepath", "-file" };
    private static readonly string[] PARAM_ENQUEUE = new string[] { "-e", "-enqueue" };
    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("wait", new Func<string, IEnumerator>(Wait));
        commandDB.AddCommand("showdialog", new Func<string[], IEnumerator>(ShowDialog));
        commandDB.AddCommand("hidedialog", new Func<string[], IEnumerator>(HideDialog));

        commandDB.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogControl));
        commandDB.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogControl));

        commandDB.AddCommand("load", new Action<string[]>(LoadDialogFile));
    }

    private static void LoadDialogFile(string[] data)
    {
        string filename = string.Empty;
        bool enqueue = false;

        var parameters = ConvertToParams(data);
        parameters.TryGetValue(PARAM_FILEPATH, out filename);
        parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultVal: false);

        string filePath = FilePaths.GetPathToResource(FilePaths.ResourcesDialogFiles, filename);
        TextAsset file = Resources.Load<TextAsset>(filePath);

        if (file == null)
        {
            Debug.LogError($"Error, el archivo {filename} no pudo ser cargado en los archivos de dialogo. Revisa el directorio de {FilePaths.ResourcesDialogFiles} para poder confirmar.");
            return;
        }

        List <string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
        Conversation newConversation = new Conversation(lines);

        if (enqueue)
            DialogController.Instance.convManager.Enqueue(newConversation);
        else
            DialogController.Instance.convManager.StartConversation(newConversation);


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
