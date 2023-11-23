using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de guardar los datos de una linea de dialogo parseada.
/// </summary>

public class DialogLineModel {
    public string speaker;
    public string dialog;
    public string commands;
    public DialogLineModel(string speaker, string dialog, string commands)
    {
        this.speaker = speaker;
        this.dialog = dialog;
        this.commands = commands;
    }   
}
