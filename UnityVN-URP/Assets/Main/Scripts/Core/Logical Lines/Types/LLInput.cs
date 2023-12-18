using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logical Line that prepares dialog for user input.
/// </summary>
public class LLInput : ILogicalLine
{
    public string Keyword => "input";
    /// <summary>
    /// Runs the input/prompt panel.
    /// </summary>
    /// <param name="line">The line that contains the input keyword.</param>
    /// <returns>The IEnumerator to be yielded.</returns>
    public IEnumerator Execute(DialogLineModel line)
    {
        string title = line.dialogData.RawData;
        PromptPanel panel = PromptPanel.Instance;
        panel.Show(title);

        while (panel.IsWaitingOnUserInput)
            yield return null;
    }

    /// <summary>
    /// Checks if matches the input keyword.
    /// </summary>
    /// <param name="line">The dialog line.</param>
    /// <returns>Wether the line matches the keyword or not.</returns>
    public bool Matches(DialogLineModel line)
    {
        return (line.HasSpeaker && line.speakerMdl.name.ToLower() == Keyword);
    }
}
