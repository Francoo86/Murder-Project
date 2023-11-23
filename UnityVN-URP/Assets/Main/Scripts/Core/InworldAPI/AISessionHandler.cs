using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class AISessionHandler
{
    private string character;
    private string sessionId;
    private string plySessionId;
    private APIClient client;
    private DateTime lastUsed;

    public string SessionId { get { return sessionId; } }
    public string PlayerSessionId { get { return plySessionId; } }

    //Tiempo maximo que se puede estar en una sesión sin realizar nada.
    private readonly float SESSION_TIMEOUT_MINUTES = 30;
    public AISessionHandler(string character, APIClient client)
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

    public async void UpdateSession() {
        if ((!IsValid()) || HasSessionExpired())
        {
            await RequestNewSession();
        }
        else 
        {
            lastUsed = DateTime.Now;
        }
    }

    public async Task<bool> RequestNewSession() {
        PlayerModel plyModel = PlayerModel.GetInstance();
        Debug.Log($"Is this an object?{plyModel}");
        
        var data = await client.RequestCharacterSession(character, plyModel.GetData());

        if (data == null) { 
            return false;
        }

        Debug.Log("Here we are in the RequestNewSession function");

        var deserialSession = JsonConvert.DeserializeObject<SessionResponseModel>((string)data);
        var charsData = deserialSession.SessionCharacters;

        //Usually the first one.
        CharacterSessionInfo Info = charsData[0];

        //Save the data in class.
        sessionId = deserialSession.Name;
        plySessionId = Info.Name;
        lastUsed = DateTime.Now;

        Debug.Log($"Data obtained from session: {sessionId}, {plySessionId}");

        return true;
    }

    /// <summary>
    /// Setter y getter para cliente utilizando un alias.
    /// </summary>
    public APIClient Client {
        get { return client; }
        set { client = value; }
    }
}
