﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CharacterInteraction
{
    private List<string> lastText;
    //For now we are reserving this.
    private string lastEmotion;
    private string emotionStrengthness;
    private const string ERROR_MESSAGE = "Sorry i'm sleeping right now, maybe try by resetting the game, or connecting to internet to re-think about myself.";

    public void SetLastInteraction(List<string> lastText, string lastEmotion, string emotionStrengthness)
    {
        Debug.Log($"Calling this method: {lastEmotion}");
        this.lastText = lastText;
        this.lastEmotion = lastEmotion;
        this.emotionStrengthness = emotionStrengthness;
    }

    /// <summary>
    /// Inyectar personaje para que pueda decir algo.
    /// </summary>
    /// <param name="character"></param>
    public IEnumerator DisplayText(Character character)
    {
        if (lastText == null) 
            yield return character.Say(ERROR_MESSAGE);
        else
            yield return character.Say(lastText);
    }


}