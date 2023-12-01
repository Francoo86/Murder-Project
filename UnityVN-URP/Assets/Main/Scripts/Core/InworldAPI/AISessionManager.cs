using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISessionManager
{
    private string character;
    private string sessionId;
    private string plySessionId;
    private APIClientV2 client;
    private DateTime lastUsed;

    public string SessionId { get { return sessionId; } }
    public string PlayerSessionId { get { return plySessionId; } }
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
    public AISessionManager(string character, APIClientV2 client)
    {
        this.character = character;
        this.client = client;
    }

    public bool IsValid() { 
        return sessionId != null;
    }

    //TODO: Test this.
    private bool HasSessionExpired() { 
        DateTime currentTime = DateTime.Now;

        if (currentTime > lastUsed.AddMinutes(SESSION_TIMEOUT_MINUTES)) { 
            lastUsed = currentTime;
            return true;
        }

        return false;
    }

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

    public IEnumerator RequestNewSession() {
        Debug.Log($"Is this an object? {client == null}");

        yield return client.RequestCharacterSession(character, PROTAGONIST_DATA, responseData => {
            var deserialSession = JsonConvert.DeserializeObject<SessionResponseModel>(responseData);
            var charsData = deserialSession.SessionCharacters;

            //Usually the first one.
            CharacterSessionInfo Info = charsData[0];

            Debug.LogWarning($"Formatting session data: {Info.Character}");
            //Save the data in class.
            sessionId = deserialSession.Name;
            plySessionId = Info.Character;
            lastUsed = DateTime.Now;

            Debug.Log($"Data obtained from session: {sessionId}, {plySessionId}");
        });
    }

    /// <summary>
    /// Setter y getter para cliente utilizando un alias.
    /// </summary>
    public APIClientV2 Client {
        get { return client; }
        set { client = value; }
    }
}
