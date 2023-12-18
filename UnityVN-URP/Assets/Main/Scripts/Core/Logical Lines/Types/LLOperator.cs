using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static LogicalLineUtils.Expressions;

/// <summary>
/// Handles the variable operations like asignation, comparison and math operations.
/// </summary>
public class LLOperator : ILogicalLine
{
    public string Keyword => throw new System.NotImplementedException();
    private const char VARIABLE_START = '$';

    /// <summary>
    /// Runs the variables considering the expression that was used, makes calculation for math operators and makes comparisons, and most important the asignation
    /// that asigns variables and saves it into VariableStore.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>The IEnumerator to be yielded.</returns>
    public IEnumerator Execute(DialogLineModel line)
    {
        string trimmedLine = line.RawData.Trim();
        string[] parts = Regex.Split(trimmedLine, REGEX_ARITHMETIC);

        if(parts.Length < 3)
        {
            Debug.LogError($"Invalid command: {trimmedLine}");
            yield break;
        }

        string variable = parts[0].Trim().TrimStart(VARIABLE_START);
        string op = parts[1].Trim();

        string[] remainingParts = new string[parts.Length - 2];
        Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

        object value = CalculateValue(remainingParts);

        if(value == null)
            yield break;

        ProcessOperator(variable, op, value);
    }

    /// <summary>
    /// Process operators defined in the line.
    /// </summary>
    /// <param name="variable">Variable name.</param>
    /// <param name="op">The operator in the line.</param>
    /// <param name="value">The value to be handled.</param>
    private void ProcessOperator(string variable, string op, object value)
    {
        if(VariableStore.TryGetValue(variable, out object currentValue))
        {
            ProcessOperatorOnVariable(variable, op, value, currentValue);
        }
        else if (op == "=")
        {
            VariableStore.CreateVariable(variable, value);
        }
    }

    /// <summary>
    /// Process operators but on a variable definition.
    /// </summary>
    /// <param name="variable">The variable (created by $variableName).</param>
    /// <param name="op">The operator assignation like =, +=, -=, *=.</param>
    /// <param name="value">The value that will be set (or reassignated.).</param>
    /// <param name="currentValue">Current value to be used for reassignation.</param>
    private void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue) {
        switch(op)
        {
            case "=":
                VariableStore.TrySetValue(variable, value); 
                break;
            case "+=":
                VariableStore.TrySetValue(variable, ConcatenateOrAdd(value, currentValue));
                break;
            case "-=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                break;
            case "/=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                break;
            case "*=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                break;
            default:
                Debug.LogError($"Operador invalido: {op}");
                break;
        }
        
    }

    /// <summary>
    /// Concatenates values if both are strings, otherwise it adds them.
    /// </summary>
    /// <param name="value">The value of the variable to add.</param>
    /// <param name="currentValue">The current value that will add the value param.</param>
    /// <returns></returns>
    private object ConcatenateOrAdd(object value, object currentValue)
    {
        if (value is string)
            return currentValue.ToString() + value;

        return Convert.ToDouble(value) + Convert.ToDouble(currentValue);
    }

    /// <summary>
    /// Checks if the operators like $, = and reassignation ones are present in the line.
    /// </summary>
    /// <param name="line">The dialog line.</param>
    /// <returns>Wether the line matches the operators.</returns>
    public bool Matches(DialogLineModel line)
    {
        Match match = Regex.Match(line.RawData.Trim(), REGEX_OPERATOR_LINE);
        return match.Success;
    }
}
