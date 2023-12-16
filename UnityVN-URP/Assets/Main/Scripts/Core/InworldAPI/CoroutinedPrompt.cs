using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CHARACTERS;

public class CoroutinePrompt {
    //Another singleton moment.
    private static CoroutinePrompt instance;
    private AISessionManager currentSession;
    private CharacterInteraction lastInteraction = new CharacterInteraction();

    private CoroutinePrompt()
    {

    }

    public static CoroutinePrompt Instance
    {
        get {
            if(instance == null)
            {
                instance = new CoroutinePrompt();
            }

            return instance;
        }
    }

    public void InjectSession(AISessionManager session)
    {
        currentSession = session;
    }

    //Void method should keep it there.
    public IEnumerator Talk(string text)
    {
        if (currentSession == null) {
            yield return null;
        }
        APIClientV2 client = currentSession.Client;

        yield return currentSession.UpdateSession();
        yield return client.SendPrompt(currentSession.SessionId, currentSession.PlayerSessionId, text, responseData => {
            var deserializedInteraction = JsonConvert.DeserializeObject<InteractionInfo>(responseData);
            if (deserializedInteraction != null)
            {
                for (int i = 0; i < deserializedInteraction.TextList.Count; i++)
                {
                    Debug.LogWarning($"Printing fetched text: {deserializedInteraction.TextList[i]}");
                }
            }

            EmotionInfo emoteInfo = deserializedInteraction.Emotion;
            lastInteraction.SetLastInteraction(deserializedInteraction.TextList, emoteInfo.Behavior, emoteInfo.Strength);
        });
    }

    public IEnumerator TestCharacter(Character character)
    {
        yield return lastInteraction.DisplayText(character);
    }
}
