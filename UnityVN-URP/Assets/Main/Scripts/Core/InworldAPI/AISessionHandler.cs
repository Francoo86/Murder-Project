using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISessionHandler
{
    private string character;
    private string sessionId;
    private string plySessionId;
    private APIClient client;
    private DateTime lastUsed;

    public AISessionHandler(string character, APIClient client)
    {
        this.character = character;
        this.client = client;
    }
}
