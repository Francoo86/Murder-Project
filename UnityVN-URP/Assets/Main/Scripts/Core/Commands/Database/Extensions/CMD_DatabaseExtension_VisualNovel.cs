using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_VisualNovel : CommandDBExtension
    {
        new public static void Extend(CommandDB database)
        {
            // Asignacion de variable.
            database.AddCommand("setplayername_variable", new Action<string>(SetPlayerNameVariable));
        }

        private static void SetPlayerNameVariable(string data)
        {
            VISUALNOVEL.VNGameSave.activeFile.playerName = data;
        }
    }
}