using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Queue for conversation that ConversationManager.
/// It gives more dynamic controlling about conversations as they are intercalating.
/// </summary>
public class ConversationQueue
{
    private Queue<Conversation> conversationQueue = new Queue<Conversation>();
    public Conversation top => conversationQueue.Peek();

    /// <summary>
    /// Adds a conversation to the queue.
    /// </summary>
    /// <param name="conversation">Conversation object</param>
    public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);

    /// <summary>
    /// Prioritizes a conversation by positioning it first.
    /// </summary>
    /// <param name="conversation">Conversation object</param>
    public void EnqueuePriority(Conversation conversation)
    {
        Queue<Conversation> queue = new Queue<Conversation>();
        queue.Enqueue(conversation);

        while(conversationQueue.Count > 0)
            queue.Enqueue(conversationQueue.Dequeue());

        conversationQueue = queue;
    }

    /// <summary>
    /// Removes last conversation from the queue.
    /// </summary>
    public void Dequeue()
    {
        if(conversationQueue.Count > 0)
        { conversationQueue.Dequeue(); }
    }

    /// <summary>
    /// Checks if the queue is empty.
    /// </summary>
    /// <returns>Wether the queue is empty or not</returns>
    public bool IsEmpty() => conversationQueue.Count == 0;
    /// <summary>
    /// Empties the queue.
    /// </summary>
    public void Clear() => conversationQueue.Clear();

    public Conversation[] GetReadOnly => conversationQueue.ToArray();
}
