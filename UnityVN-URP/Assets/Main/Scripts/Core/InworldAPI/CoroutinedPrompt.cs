using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CHARACTERS;

public class CoroutinePrompt {
    //Another singleton moment.
    private static CoroutinePrompt instance;
    private AISessionManager currentSession;
    private CharacterInteraction lastInteraction;
    public bool IsWaiting = false;
    public bool IsStillFetching => currentSession.Client.IsFetching;
    public bool IsTalkingWithCharacter = false;
    public Conversation savedConversation = null;
    public Character lastCharacter = null;

    /// <summary>
    /// Initializes the Coroutined Prompt class, in singleton way.
    /// </summary>
    private CoroutinePrompt() {
        lastInteraction = new CharacterInteraction();
    }

    /// <summary>
    /// Get the current instance of this class.
    /// </summary>
    /// <returns>The instance.</returns>
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

    private ConversationManager convManager => DialogController.Instance.convManager;

    public void BackupLastConversation()
    {
        Conversation[] allConvs = convManager.GetConversationQueue();
        savedConversation = allConvs[allConvs.Length - 1];
    }

    public void RestoreLastConversation()
    {
        convManager.StartConversation(savedConversation);
    }

    //Void method should keep it there.
    public IEnumerator Talk(string text)
    {
        if (currentSession == null) {
            yield return null;
        }
        InworldWrapper client = currentSession.Client;

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

    //Esta funcion fue cambiada debido a que los personajes con la función Say reinician la conversación y encolan la conversación de inworld.
    /// <summary>
    /// Makes the character dialog box display the resultant response text and saves it into the actual conversation.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    public IEnumerator Interact(Character character)
    {
        lastCharacter = character;
        yield return lastInteraction.DisplayText(character);
    }

    //public string GetResponseExpression() => lastInteraction.GetLastEmotion();
}
