using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Retrieves all the info associated to Speaker (character), DialogData (the line with commands like wait), Commands (custom methods defined to extend functionality).
/// Associated to one line.
/// </summary>
public class DialogLineModel {
    //public string speaker;
    public string RawData { get; private set; } = string.Empty;
    public SpeakerModel speakerMdl;
    public DialogData dialogData;
    public CommandData commandData;

    public bool HasDialog => (dialogData != null && dialogData.HasDialog);//=> dialog != string.Empty;
    public bool HasCommands => commandData != null;//commands != string.Empty;
    public bool HasSpeaker => speakerMdl != null;//speaker != string.Empty;
    /// <summary>
    /// Creates the Dialog line model to be used with Conversation Manager.
    /// </summary>
    /// <param name="rawData">The raw line.</param>
    /// <param name="speaker">The speaker retreived by the parser.</param>
    /// <param name="dialog">The dialog line (inside in 'dialog some test').</param>
    /// <param name="commands">The command if the line has any.</param>
    public DialogLineModel(string rawData, string speaker, string dialog, string commands)
    {
        RawData = rawData;
        speakerMdl = (!string.IsNullOrEmpty(speaker) ? new SpeakerModel(speaker) : null);
        dialogData = (!string.IsNullOrEmpty(dialog) ? new DialogData(dialog) : null);
        commandData = (!string.IsNullOrEmpty(commands) ? new CommandData(commands) : null);//commands;
    }   
}
