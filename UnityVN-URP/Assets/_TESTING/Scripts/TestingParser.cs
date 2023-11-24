using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SendFileToParser();
    }

    // Update is called once per frame
    void SendFileToParser()
    {

        List<string> lines = FileManager.ReadTextFile("dialogspeaker.txt");

        //Command testing.

        foreach (string line in lines) { 
            if(string.IsNullOrWhiteSpace(line)) continue;

            Debug.Log($"{line}");

            DialogLineModel dialogLine = DialogParser.Parse(line);

            for(int i = 0; i < dialogLine.commandData.commands.Count; i++) {
                CommandData.Command command = dialogLine.commandData.commands[i];
                Debug.Log($"Command [{i}] '{command.name}' has arguments like [{string.Join(", ", command.arguments)}]");
            }
        }
        DialogController.Instance.Say(lines);
        /*
        DialogController.Instance.Say(lines);
        foreach (string line in lines) {
            Debug.Log($"Trying to parsing line: {line}");
            DialogLineModel dialogLine = DialogParser.Parse(line);

            int i = 0;
            foreach(DialogData.DIALOG_SEGMENT segment in dialogLine.dialog.segments)
            {
                if (string.IsNullOrEmpty(line)) continue;

                Debug.Log($"Segment [{i++}] = '{segment.dialog}' " +
                    $"[signal={segment.startSignal}{(segment.signalDelay > 0 ? $" {segment.signalDelay}" : $"")}]");
            }
        }*/

    }
}
