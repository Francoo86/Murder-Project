using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISessionManager
{
    private string character;
    private DateTime lastUsed;

    public InworldWrapper Client => InworldWrapper.Instance;

    public string SessionId { get; private set; }
    public string PlayerSessionId { get; private set; }
    //Maybe it can used by another classes.
    public static readonly Dictionary<string, string> PROTAGONIST_DATA = new Dictionary<string, string>()
        {
            { "givenName", "Luis" },
            {"age", "30" },
            {"gender", "male" },
            {"role", "detective" }
        };

    //Tiempo maximo que se puede estar en una sesión sin realizar nada.
    private readonly float SESSION_TIMEOUT_MINUTES = 30;
    /// <summary>
    /// Initializes the session manager.
    /// </summary>
    /// <param name="character">The character we are going to use.</param>
    public AISessionManager(string character)
    {
        this.character = character;

    }

    /// <summary>
    /// Checks if current session is not null.
    /// </summary>
    /// <returns>Wether the session is valid or not.</returns>
    public bool IsValid() { 
        return SessionId != null;
    }

    /// <summary>
    /// Checks if the session surpassed the 30 min AFK threshold.
    /// </summary>
    /// <returns>The check.</returns>
    private bool HasSessionExpired() { 
        DateTime currentTime = DateTime.Now;

        if (currentTime > lastUsed.AddMinutes(SESSION_TIMEOUT_MINUTES)) { 
            lastUsed = currentTime;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Updates the session time.
    /// </summary>
    /// <returns>The IEnumerator that yields the session request.</returns>
    public IEnumerator UpdateSession() {
        if ((!IsValid()) || HasSessionExpired())
        {
            yield return RequestNewSession();
        }
        else 
        {
            lastUsed = DateTime.Now;
        }
    }

    /// <summary>
    /// Requests a new session and saves it.
    /// </summary>
    /// <returns>Current IEnumerator</returns>
    public IEnumerator RequestNewSession() {
        yield return Client.RequestCharacterSession(character, PROTAGONIST_DATA, FetchSession);
    }

    /// <summary>
    /// Internal function that fetchs the session.
    /// </summary>
    /// <param name="responseData">The data fetched from Inworld.</param>
    private void FetchSession(string responseData)
    {
        var deserialSession = JsonConvert.DeserializeObject<SessionResponseModel>(responseData);
        var charsData = deserialSession.SessionCharacters;

        //Usually the first one.
        CharacterSessionInfo Info = charsData[0];

        Debug.LogWarning($"Formatting session data: {Info.Character}");
        //Save the data in class.
        SessionId = deserialSession.Name;
        PlayerSessionId = Info.Character;
        lastUsed = DateTime.Now;

        Debug.Log($"Data obtained from session: {SessionId}, {PlayerSessionId}");
    }
}
