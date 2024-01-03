using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class that holds all the info about the speaker found in the dialog.
/// Saves things like expression, pos, casting name, name.
/// </summary>
public class SpeakerModel
{
    public string RawData { get; private set; } = string.Empty;

    public string name, screenName;
    //Posicion del personaje en pantalla, tal como RenPy.
    public Vector2 speakerScrPos;
    //Como se mostrará el personaje en pantalla.
    public string DisplayName => IsScreenName ? screenName : name;
    public bool IsScreenName => screenName != string.Empty;
    public bool IsGoingToScreenPos => false;
    public bool IsDoingAnyExpression => actualExpression != "";//ScreenExpressions.Count > 0;
    //Las emociones son papel fundamental en una novela.
    //Por lo que veo estas son capas, lo más probable es que solo usaremos la primera solamente.
    public string actualExpression = "";//List<(int layer, string expression)> ScreenExpressions { get; set; }

    //Minuto 13.
    public bool MakeCharacterEnter = false;

    //Poner constantes para poder parsear bien los datos.
    private const string SCREENNAME_ID = " as ";
    private const string POSITION_ID = " at ";
    private const string EXPRESSION_ID = " [";
    //Para la position en pantalla, asumiendo que [1:1] es x:y;
    private const char AXISDELIMITER_ID = ':';
    private const char EXPRESSIONLAYER_JOINER = ',';
    private const char EXPRESSIONLAYER_DELIMITER = ':';
    //Palabra clave para mostrar al personaje en pantalla.
    private const string SHOWCHARACTER_ID = "enter ";
    /// <summary>
    /// Initializes the object and tries to match the speaker data, like enter command or casting name.
    /// </summary>
    /// <param name="speaker">Raw name of the speaker and its commands.</param>
    public SpeakerModel(string speaker) {
        RawData = speaker;
        InitializeSpeakerModel();
        Debug.Log($"Getting the speaker {speaker}");
        //Revisar si se encuentran estos "comandos".
        MatchSpeakerData(speaker);
    }

    /// <summary>
    /// Initializes placeholder data to avoid problems with null references.
    /// </summary>
    private void InitializeSpeakerModel() {
        //Inicializar para evitar problemas de referencias.
        screenName = "";
        speakerScrPos = Vector2.zero;
        actualExpression = "normal"; ///new List<(int layer, string expression)>();
    }

    //The constructor does too much work.
    //FIXME: Refactor pls.
    /// <summary>
    /// Process keywords for showing.
    /// </summary>
    /// <param name="speaker">Raw name of the speaker</param>
    /// <returns>The speaker without the enter command</returns>
    private string ProcessKeywords(string speaker)
    {
        if (speaker.StartsWith(SHOWCHARACTER_ID))
        {
            speaker = speaker.Substring(SHOWCHARACTER_ID.Length);
            MakeCharacterEnter = true;
        }

        return speaker;
    }

    //I hate many parameters but this shit is worth of this.
    /// <summary>
    /// Makes the full match logic for getting the character casting and handling commands, like expression, pos, casting.
    /// </summary>
    /// <param name="speaker">Raw speaker with the logic.</param>
    private void MatchSpeakerData(string speaker = "") {
        speaker = ProcessKeywords(speaker);

        string speakerPattern = @$"{SCREENNAME_ID}|{POSITION_ID}|{EXPRESSION_ID.Insert(EXPRESSION_ID.Length - 1, @"\")}";
        MatchCollection matches = Regex.Matches(speaker, speakerPattern);

        if (matches.Count == 0)
        {
            name = speaker;
            return;
        }

        int index = matches[0].Index;
        name = speaker.Substring(0, index);
        //name = speaker;
        for (int i = 0; i < matches.Count; i++)
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
            else if (match.Value == EXPRESSION_ID)
            {
                startIdx = match.Index + EXPRESSION_ID.Length;
                endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : speaker.Length;
                string castedExpr = speaker.Substring(startIdx, endIdx - (startIdx + 1));

                //Debug.Log("TAK TAK");
                //Debug.Log(castedExpr);
                actualExpression = castedExpr;

                /*
                ScreenExpressions = castedExpr.Split(EXPRESSIONLAYER_JOINER)
                    //Very JS syntax to forEach.
                    .Select(elem => {
                        Debug.Log($"{elem}");
                        var parts = elem.Trim().Split(EXPRESSIONLAYER_DELIMITER);

                        Debug.Log($"EXPRESSION GOT: {parts[0]}");

                        if (parts.Length == 2)
                            return (int.Parse(parts[0]), parts[1]);
                        else
                            return (0, parts[0]);
                    }).ToList();*/
            }
        }
    }
}
