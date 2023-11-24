using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SpeakerModel
{
    // Start is called before the first frame update
    public string name, screenName;
    //Posicion del personaje en pantalla, tal como RenPy.
    public Vector2 speakerScrPos;
    //Como se mostrará el personaje en pantalla.
    public string DisplayName => screenName ?? name;
    //Las emociones son papel fundamental en una novela.
    public List<(int layer, string expression)> ScreenExpressions { get; set; }

    //Poner constantes para poder parsear bien los datos.
    private const string SCREENNAME_ID = " as ";
    private const string POSITION_ID = " at ";
    private const string EXPRESSION_ID = " [";
    //Para la position en pantalla, asumiendo que [1:1] es x:y;
    private const char AXISDELIMITER_ID = ':';
    private const char EXPRESSIONLAYER_JOINER = ',';
    private const char EXPRESSIONLAYER_DELIMITER = ':';
    public SpeakerModel(string speaker) {
        //Revisar si se encuentran estos "comandos".
        string speakerPattern = @$"{SCREENNAME_ID}|{POSITION_ID}|{EXPRESSION_ID.Insert(EXPRESSION_ID.Length - 1, @"\")}";
        MatchCollection matches = Regex.Matches(speaker, speakerPattern);

        //Inicializar para evitar problemas de referencias.
        screenName = "";
        speakerScrPos = Vector2.zero;
        ScreenExpressions = new List<(int layer, string expression)>();

        //Nada que hacer ya que solo tenemos al personaje.
        if (matches.Count == 0)
        {
            name = speaker;
            return;
        }

        int index = matches[0].Index;
        name = speaker.Substring(0, index);
        //name = speaker;
        for(int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            int startIdx = 0, endIdx = 0;

            //TODO: Refactor.
            if (match.Value == SCREENNAME_ID)
            {
                startIdx = match.Index + SCREENNAME_ID.Length;
                endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : speaker.Length;
                screenName = speaker.Substring(startIdx, endIdx - startIdx);
            }
            else if (match.Value == POSITION_ID)
            {
                startIdx = match.Index + POSITION_ID.Length;
                endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : speaker.Length;
                string scrPos = speaker.Substring(startIdx, endIdx - startIdx);

                string[] axis = scrPos.Split(AXISDELIMITER_ID, System.StringSplitOptions.RemoveEmptyEntries);

                float.TryParse(axis[0], out speakerScrPos.x);

                if (axis.Length > 1)
                {
                    float.TryParse(axis[1], out speakerScrPos.y);
                }
            }
            else if (match.Value == EXPRESSION_ID) {
                startIdx = match.Index + EXPRESSION_ID.Length;
                endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : speaker.Length;
                string castedExpr = speaker.Substring(startIdx, endIdx - (startIdx + 1));

                ScreenExpressions = castedExpr.Split(EXPRESSIONLAYER_JOINER)
                    //Very JS syntax to forEach.
                    .Select(elem => {
                        var parts = elem.Trim().Split(EXPRESSIONLAYER_DELIMITER);
                        //Debug.Log($"Parts of this thing: {parts[0]}, {parts[1]}.");
                        return (int.Parse(parts[0]), parts[1]);
                    }).ToList();
            }
        }
    }
}
