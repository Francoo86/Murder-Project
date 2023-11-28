using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptSender {
    //Another singleton moment.
    private static PromptSender instance;
    private AISessionHandler currentSession;
    private CharacterInteraction lastInteraction = new CharacterInteraction();

    private PromptSender()
    {

    }

    public static PromptSender Instance
    {
        get {
            if(instance == null)
            {
                instance = new PromptSender();
            }

            return instance;
        }
    }

    public void InjectSession(AISessionHandler session)
    {
        currentSession = session;
    }

    //Void method should keep it there.
    public async void Talk(string text)
    {
        if (currentSession == null) {
            return;
        }

        currentSession.UpdateSession();

        APIClient client = currentSession.Client;
        var data = await client.SendPrompt(currentSession.SessionId, currentSession.PlayerSessionId, text);

        if (data == null) return;
        var deserializedInteraction = JsonConvert.DeserializeObject<InteractionInfo>((string)data);
        if (deserializedInteraction != null)
        {
            for(int i = 0; i < deserializedInteraction.TextList.Count; i++)
            {
                Debug.LogWarning($"Printing fetched text: {deserializedInteraction.TextList[i]}");
            }
        }

        EmotionInfo emoteInfo = deserializedInteraction.Emotion;
        lastInteraction.SetLastInteraction(deserializedInteraction.TextList, emoteInfo.Behavior, emoteInfo.Strength);
    }

    public IEnumerator TestCharacter(Character character)
    {
        yield return lastInteraction.DisplayText(character);
    }
}
