﻿using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;
class CharacterInteraction
{
    private List<string> lastText;
    //For now we are reserving this.
    private string lastEmotion;
    private string emotionStrengthness;

    private Dictionary<string, string> ACCEPTED_EMOTIONS = new Dictionary<string, string>
    {
        {"NEUTRAL", "normal"},
        {"ANGER", "enojado"},
        {"JOY", "feliz"},
        {"TENSION", "pensativo"},
        {"SADNESS", "triste"},
        {"SURPRISE", "sorprendido"},
        {"DISGUST", "enojado"},
        {"CONTEMPT", "enojado"},
        {"BELLIGERENCE", "enojado"},
        {"DOMINEERING", "enojado"},
        {"CRITICISM", "enojado"},
        {"TENSE_HUMOR", "pensativo"},
        {"DEFENSIVENESS", "enojado"},
        {"WHINING", "triste"},
        {"STONEWALLING", "pensativo"},
        {"INTEREST", "normal"},
        {"VALIDATION", "feliz"},
        {"AFFECTION", "feliz"},
        {"HUMOR", "feliz"},
        {"SPAFF_CODE_UNSPECIFIED", "normal"},
    };

    private const string ERROR_MESSAGE = "Sorry i'm sleeping right now, maybe try by resetting the game, or connecting to internet to re-think about myself.";

    /// <summary>
    /// Saves the last interaction to be displayed on the screen.
    /// </summary>
    /// <param name="lastText">Last conversation.</param>
    /// <param name="lastEmotion">Last emotion.</param>
    /// <param name="emotionStrengthness">The last emotion strength.</param>
    public void SetLastInteraction(List<string> lastText, string lastEmotion, string emotionStrengthness)
    {
        //Debug.Log($"Calling this method: {lastEmotion}");
        this.lastText = lastText;

        //To parse the interaction correctly.
        if (ACCEPTED_EMOTIONS.ContainsKey(lastEmotion))
            lastEmotion = ACCEPTED_EMOTIONS[lastEmotion];

        this.lastEmotion = lastEmotion;
        this.emotionStrengthness = emotionStrengthness;
    }

    /// <summary>
    /// Inyectar personaje para que pueda decir algo.
    /// </summary>
    /// <param name="character"></param>
    public IEnumerator DisplayText(Character character)
    {
        Debug.Log($"Expression received: {lastEmotion}");
        character.OnExpressionReceive(lastEmotion ?? "normal");
        yield return character.Say(lastText);
    }
}
