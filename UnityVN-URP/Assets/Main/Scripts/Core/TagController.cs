using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static LogicalLineUtils.Expressions;

/// <summary>
/// Class that holds the logic of how to parse dynamic variables. Like tags of HTML or variables defined with the dollar sign.
/// </summary>
public class TagController
{   
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>() {
        { "<playerName>", () => "Juanito" },
        { "<time>", () => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") },
        { "<tempVal>", () => "test2" },
        { "<input>", () => PromptPanel.Instance.LastInput }
    };

    private static readonly Regex regPattern = new Regex("<\\w+>");
    /// <summary>
    /// Makes the fully injection of variables of any type (if needed).
    /// </summary>
    /// <param name="text">The line with the variables.</param>
    /// <param name="injectTags">Inject tags like XML or HTML ones, these should be defined above.</param>
    /// <param name="injectVariables">Inject variables with the dollar sign (these are more dynamic).</param>
    /// <returns>The parsed text without the variables.</returns>
    public static string Inject(string text, bool injectTags = true, bool injectVariables = true)
    {
        if(injectTags)
            text = InjectTags(text);

        if(injectVariables)
            text = InjectVariables(text);

        return text;
    }

    /// <summary>
    /// Internal method used by Inject for parsing the variable tags and executing the code associated with the tag.
    /// </summary>
    /// <param name="text">Line with the variables.</param>
    /// <returns>Line without the variables.</returns>

    private static string InjectTags(string text)
    {
        if (regPattern.IsMatch(text))
        {
            foreach(Match match in regPattern.Matches(text))
            {
                if(tags.TryGetValue(match.Value, out var tagRequest))
                {
                    text = text.Replace(match.Value, tagRequest());
                }
            }
        }

        return text;
    }

    /// <summary>
    /// Internal method used by Inject por parsing the dynamic variables like ($variable) and trying to store them in the VariableStore class.
    /// </summary>
    /// <param name="value">The line to parse and get the variables.</param>
    /// <returns>The text parsed without the variables.</returns>
    private static string InjectVariables(string value)
    {
        var matches = Regex.Matches(value, REGEX_VARIABLE_IDS);
        var matchList = matches.Cast<Match>().ToList();

        for(int i = matchList.Count - 1; i >= 0; i--)
        {
            var match = matchList[i];
            string variableName = match.Value.TrimStart(VARIABLE_ID, NEGATED_ID);
            bool negated = match.Value.StartsWith(NEGATED_ID);

            bool endsInIllegalCharacter = variableName.EndsWith(VariableStore.DATABASE_RELATIONAL_ID);

            if(endsInIllegalCharacter)
                variableName = variableName.Substring(0, variableName.Length - 1);

            if(!VariableStore.TryGetValue(variableName, out var variableValue))
            {
                Debug.LogError($"La variable {variableName} no fue encontrada en la asignación.");
                continue;
            }

            if (negated && variableValue is bool)
                variableValue = !(bool)variableValue;

            int lengthToBeRemoved = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;

            if (endsInIllegalCharacter)
                lengthToBeRemoved -= 1;

            value = value.Remove(match.Index, lengthToBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
        }

        return value;
    }
}
