using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


/// 
/// <summary>
/// Se encaraga del dialogo e intenta parsear las funciones escritas dentro de la misma linea.
/// </summary>
public class DialogParser {
    //Buscamos palabras, pero sin espacios en blanco para realizar el patrón.
    private const string cmdCommandExp = "\\w*[^\\s]\\(";
    public static DialogLineModel Parse(string line) {
        //Debug.Log($"Parsing this line {line}");
        (string speaker, string dialogue, string commands) = ExtractContent(line);

        //Debug.Log($"Speaker is {speaker}, dialog is {dialogue}, command is {commands}");

        return new DialogLineModel(speaker, dialogue, commands);
    }

    private static (string, string, string) ExtractContent(string line)
    {
        string speaker = "", dialogue = "", commands = "";
        int dialogStart = -1, dialogEnd = -1;
        bool isEscaped = false;

        for (int i = 0; i < line.Length; i++) { 
            char currentChar = line[i];

            //Revisa si hay texto posible para poder escaparlo.
            if (currentChar == '\\') 
                isEscaped = !isEscaped;
            else if (currentChar == '"' && !isEscaped)
            {
                if (dialogStart == -1)
                {
                    dialogStart = i;
                }
                else if (dialogEnd == -1)
                {
                    dialogEnd = i;
                }
            }
            else isEscaped = false;
        }

        //Debug.Log(line.Substring(dialogStart + 1, (dialogEnd - dialogStart) - 1));

        Regex cmdRegex = new Regex(cmdCommandExp);
        Match match = cmdRegex.Match(line);
        int commandStart = -1;

        if(match.Success)
        {
            commandStart = match.Index;

            if (dialogStart == -1 && dialogEnd == -1)
                return ("", "", line.Trim());
        }

        //Revisamos si el comando está después del dialogo.
        //Juanito "Sample Text" Bailar([comando style linux])
        if (dialogStart != -1 && dialogEnd != -1 && (commandStart == -1 || commandStart > dialogEnd))
        {
            speaker = line.Substring(0, dialogStart).Trim();
            dialogue = line.Substring(dialogStart + 1, dialogEnd - dialogStart - 1).Replace("\\\"", "\"");

            if (commandStart != -1)
                commands = line.Substring(commandStart).Trim();
        }
        else if (commandStart != -1 && dialogStart > commandStart)
            commands = line;
        else
            speaker = line;

        return (speaker,  dialogue, commands);
    }
}
