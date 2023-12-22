using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the commands created inside the Extend method of the extensions.
/// </summary>
public class CommandDB
{
    private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();
    /// <summary>
    /// Checks if the command name is on the database.
    /// </summary>
    /// <param name="commandName">The name of command</param>
    /// <returns>Wether the command exists or not.</returns>
    public bool HasCommand(string commandName) => database.ContainsKey(commandName);
    /// <summary>
    /// Adds a command into the command database with its associate function (delegate).
    /// An analogy of this is Counter-Strike, where you put commands like sv_cheats and you put 1, and that internally calls a function
    /// so this is the same.
    /// </summary>
    /// <param name="commandName">The command name.</param>
    /// <param name="command">The function that the command will execute.</param>
    public void AddCommand(string commandName, Delegate command)
    {
        commandName = commandName.ToLower();
        if (!database.ContainsKey(commandName)) database[commandName] = command;
        else Debug.LogWarning($"{commandName} command already exists on the database!");
    }
    
    /// <summary>
    /// Gets the command function with its name.
    /// </summary>
    /// <param name="commandName">Command name</param>
    /// <returns>The function that holds the command name.</returns>
    public Delegate GetCommand(string commandName)
    {
        commandName = commandName.ToLower();

        if (!HasCommand(commandName)) {
            Debug.LogWarning($"The {commandName} command doesn't exist on the database.");
            return null;
        };

        return database[commandName];
    }
}
