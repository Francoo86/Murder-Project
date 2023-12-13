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

    public LogicalLineManager() {
        LoadLogicalLines();
    }

    private void LoadLogicalLines()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] lineTypes = assembly.GetTypes().Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface).ToArray();

        foreach (Type lineType in lineTypes)
        {
            ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
            logicalLines.Add(line);
        }
    }
    public bool TryGetLogic(DialogLineModel line, out Coroutine logic)
    {
        foreach(var logicalLine in logicalLines)
        {
            if (logicalLine.Matches(line))
            {
                logic = DiagController.StartCoroutine(logicalLine.Execute(line));
            }
        }

        logic = null;
        return false;
    }
}
