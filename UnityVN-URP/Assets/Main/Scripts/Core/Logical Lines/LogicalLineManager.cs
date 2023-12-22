using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class LogicalLineManager
{
    private DialogController DiagController => DialogController.Instance;
    private List<ILogicalLine> logicalLines = new List<ILogicalLine>();

    /// <summary>
    /// Initializes the LogicalLineManager by loading logical line classes.
    /// </summary>
    public LogicalLineManager() {
        LoadLogicalLines();
    }

    /// <summary>
    /// Loads logical line by instancing each class with associated ILogicalLine interface, and saves it on the logicalLines list
    /// to be used for getting the logic.
    /// </summary>
    private void LoadLogicalLines()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        //Busca las clases que pertencen a la interfaz de ILogicalLine.
        Type[] lineTypes = assembly.GetTypes().Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface).ToArray();

        foreach (Type lineType in lineTypes)
        {
            ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
            logicalLines.Add(line);
        }
    }
    /// <summary>
    /// Tries to get the logic stuff on the line with matching it with one of the available LogicalLines
    /// and returns a Coroutine that handles that line logic.
    /// </summary>
    /// <param name="line">The dialog line.</param>
    /// <param name="logic">The process that will be run to handle the selected logical line</param>
    /// <returns></returns>
    public bool TryGetLogic(DialogLineModel line, out Coroutine logic)
    {
        foreach(var logicalLine in logicalLines)
        {
            if (logicalLine.Matches(line))
            {
                logic = DiagController.StartCoroutine(logicalLine.Execute(line));
                return true;
            }
        }

        logic = null;
        return false;
    }
}
