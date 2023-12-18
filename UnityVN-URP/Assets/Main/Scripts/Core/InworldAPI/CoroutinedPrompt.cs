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

    /// <summary>
    /// Initializes the Coroutined Prompt class, in singleton way.
    /// </summary>
    private CoroutinePrompt() {}

    public static CoroutinePrompt GetInstance()
    {
        if(instance == null)
        {
            instance = new CoroutinePrompt();
        }

        return instance;
    }

    /// <summary>
    /// Sets the current session to be used with the instance.
    /// </summary>
    /// <param name="session">The session manager.</param>
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
        yield return client.SendPrompt(currentSession.SessionId, currentSession.PlayerSessionId, text, FetchCharacterResponse);
    }

    /// <summary>
    /// Fetches the character response with their feeling and the speech.
    /// </summary>
    /// <param name="responseData">The received data.</param>
    private void FetchCharacterResponse(string responseData)
    {
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
    }

    public IEnumerator TestCharacter(Character character)
    {
        yield return lastInteraction.DisplayText(character);
    }
}
