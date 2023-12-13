using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Clase encargada de identificar las variables dinamicas que se usaran para el juego.
/// </summary>
public class TagController
{
    private readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>();
    private readonly Regex regPattern = new Regex("<\\w+>");

    public TagController() {
        InitializeTags();
    }

    //TODO: Ajustarlo a los requisitos.
    private void InitializeTags()
    {
        //Harcoded af.
        tags["<playerName>"] = () => "Juanito";
        tags["<time>"] = () => DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        tags["<tempVal>"] = () => "test2";
        tags["<input>"] = () => PromptPanel.Instance.LastInput;
    }

    public string InjectTags(string text)
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
}
