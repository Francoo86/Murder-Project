using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Linea logica que acepta inputs de usuario.
/// </summary>
public class LLInput : ILogicalLine
{
    public string Keyword => "input";
    public IEnumerator Execute(DialogLineModel line)
    {
        string title = line.dialogData.RawData;
        PromptPanel panel = PromptPanel.Instance;
        panel.Show(title);

        while (panel.IsWaitingOnUserInput)
            yield return null;
    }

    public bool Matches(DialogLineModel line)
    {
        return (line.HasSpeaker && line.speakerMdl.name.ToLower() == Keyword);
    }
}
