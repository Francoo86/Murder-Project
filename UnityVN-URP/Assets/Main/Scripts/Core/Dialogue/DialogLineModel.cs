using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de guardar los datos de una linea de dialogo parseada.
/// </summary>

public class DialogLineModel {
    public string speaker;
    public DialogData dialog;
    public string commands;

    public bool HasDialog => dialog.HasDialog;//=> dialog != string.Empty;
    public bool HasCommands => commands != string.Empty;
    public bool HasSpeaker => speaker != string.Empty;
    public DialogLineModel(string speaker, string dialog, string commands)
    {
        this.speaker = speaker;
        this.dialog = new DialogData(dialog);
        this.commands = commands;
    }   
}
