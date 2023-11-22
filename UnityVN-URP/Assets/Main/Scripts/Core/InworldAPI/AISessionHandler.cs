using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AISessionHandler
{
    private string character;
    private string sessionId;
    private string plySessionId;
    private APIClient client;
    private DateTime lastUsed;

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

    public void UpdateSession() {
        if ((!IsValid()) || HasSessionExpired())
        {
            RequestNewSession();
        }
        else 
        {
            lastUsed = DateTime.Now;
        }
    }

    public async void RequestNewSession() {
        string data = (string)await client.RequestCharacterSession(character, PlayerModel.GetInstance().GetData());

        if (data == null) { 
            return;
        }

        Debug.Log("Here we are in the RequestNewSession function");

        var deserialSession = JsonConvert.DeserializeObject<SessionResponseModel>(data);
        var charsData = deserialSession.SessionCharacters;

        //Usually the first one.
        CharacterSessionInfo Info = charsData[0];

        //Save the data in class.
        sessionId = deserialSession.Name;
        plySessionId = Info.Name;
        lastUsed = DateTime.Now;
    }

    /// <summary>
    /// Setter y getter para cliente utilizando un alias.
    /// </summary>
    public APIClient Client {
        get { return client; }
        set { client = value; }
    }
}
