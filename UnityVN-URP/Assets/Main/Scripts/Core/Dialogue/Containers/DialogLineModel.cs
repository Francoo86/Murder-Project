using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de guardar los datos de una linea de dialogo parseada.
/// </summary>

public class DialogLineModel {
    //public string speaker;
    public SpeakerModel speakerMdl;
    public DialogData dialogData;
    public CommandData commandData;

    public bool HasDialog => (dialogData != null && dialogData.HasDialog);//=> dialog != string.Empty;
    public bool HasCommands => commandData != null;//commands != string.Empty;
    public bool HasSpeaker => speakerMdl != null;//speaker != string.Empty;
    public DialogLineModel(string speaker, string dialog, string commands)
    {
        speakerMdl = (!string.IsNullOrEmpty(speaker) ? new SpeakerModel(speaker) : null);
        dialogData = (!string.IsNullOrEmpty(dialog) ? new DialogData(dialog) : null);
        commandData = (!string.IsNullOrEmpty(commands) ? new CommandData(commands) : null);//commands;
    }   
}
