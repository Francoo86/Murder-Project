using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDB
{
    private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();
    public bool HasCommand(string commandName) => database.ContainsKey(commandName);
    public void AddCommand(string commandName, Delegate command)
    {
        commandName = commandName.ToLower();
        if (!database.ContainsKey(commandName)) database[commandName] = command;
        else Debug.LogWarning($"{commandName} already exists on the database!");
    }
    
    public Delegate GetCommand(string commandName)
    {
        commandName = commandName.ToLower();

        if (!HasCommand(commandName)) {
            Debug.LogWarning($"The {commandName} doesn't exist on the database.");
            return null;
        };

        return database[commandName];
    }
}
