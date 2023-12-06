using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CommandData
{
    public List<Command> commands;
    private const char COMMANDSPLITTER_ID = ',';
    private const char ARGUMENTSCONTAINER_ID = '(';
    private const string WAIT_ID = "[wait]";
    public struct Command {
        public string name;
        public string[] arguments;
        public bool waitToFinish;
    }

    //SIMILAR TO SOME PATTERN.
    //Prolly strategy.
    public CommandData(string rawCommands) {
        commands= ExtractCommands(rawCommands);
    }

    private List<Command> ExtractCommands(string rawCommands)
    {
        string[] data = rawCommands.Split(COMMANDSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
        List <Command> res = new List<Command>();

        foreach (string cmd in data) {
            Command fullCommand = new Command();
            int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
            fullCommand.name = cmd.Substring(0, index).Trim();

            //Marca al comando como debe esperar.
            if (fullCommand.name.ToLower().StartsWith(WAIT_ID))
            {
                //Quitar el identificador de espera.
                fullCommand.name = fullCommand.name.Substring(WAIT_ID.Length);
                Debug.Log($"Full command name {fullCommand.name}");
                fullCommand.waitToFinish = true;
            }
            else
                fullCommand.waitToFinish = false;

            fullCommand.arguments = GetArguments(cmd.Substring(index + 1, cmd.Length - index - 2));
            res.Add(fullCommand);
        }

        return res;
    }

    private string[] GetArguments(string args) {
        List<string> argsList = new List<string>();
        //StringBuilder para poder modificar directamente la referencia de un string.
        StringBuilder currentArg = new StringBuilder();
        bool insideQuotes = false;

        for(int i = 0; i < args.Length; i++)
        {
            char character = args[i];

            if(character == '"')
            {
                insideQuotes = !insideQuotes;
                continue;
            }

            if (!insideQuotes && character == ' ') {
                argsList.Add(currentArg.ToString());
                currentArg.Clear();
                continue;
            }

            currentArg.Append(character);
        }

        if (currentArg.Length > 0)
            argsList.Add (currentArg.ToString());

        return argsList.ToArray();
    }
}
