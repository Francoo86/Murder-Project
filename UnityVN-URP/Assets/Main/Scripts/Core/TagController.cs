using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static LogicalLineUtils.Expressions;

/// <summary>
/// Clase encargada de identificar las variables dinamicas que se usaran para el juego.
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

    //TODO: Ajustarlo a los requisitos.
    /*
    private void InitializeTags()
    {
        //Harcoded af.
        tags["<playerName>"] = () => "Juanito";
        tags["<time>"] = () => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        tags["<tempVal>"] = () => "test2";
        tags["<input>"] = () => PromptPanel.Instance.LastInput;
    }*/
    public static string Inject(string text, bool injectTags = true, bool injectVariables = true)
    {
        if(injectTags)
            text = InjectTags(text);

        if(injectVariables)
            text = InjectVariables(text);

        return text;
    }

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
