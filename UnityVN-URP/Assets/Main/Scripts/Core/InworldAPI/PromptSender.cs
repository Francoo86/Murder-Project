using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptSender {
    //Another singleton moment.
    private static PromptSender instance;
    private AISessionHandler currentSession;

    private PromptSender()
    {

    }

    public void InjectSession(AISessionHandler session)
    {
        currentSession = session;
    }

    public async void Talk(string text)
    {
        if (currentSession == null) {
            return;
        }

        currentSession.UpdateSession();

        APIClient client = currentSession.Client;
        var res = await client.SendPrompt(currentSession.SessionId, currentSession.PlayerSessionId, text);
    }
}
