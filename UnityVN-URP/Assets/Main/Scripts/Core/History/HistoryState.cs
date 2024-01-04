using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace History
{
    /// <summary>
    /// Saves the current state of the history, this means it saves the characters, data, audio, where they
    /// have in that moment.
    /// </summary>
    [System.Serializable]
    public class HistoryState
    {
        public DialogueData dialogue;
        public List<CharacterData> characters;
        public List<AudioData> audio;
        public List<GraphicData> graphics;

        /// <summary>
        /// Captures all the data of the moment. (dialog, audio, characters, graphics).
        /// </summary>
        /// <returns>The history state with the requested data captured.</returns>
        public static HistoryState Capture() 
        {
            HistoryState state = new HistoryState();
            state.dialogue = DialogueData.Capture();
            state.characters = CharacterData.Capture();
            state.audio = AudioData.Capture(); 
            state.graphics = GraphicData.Capture();  
            return state;
        }

        /// <summary>
        /// Loads the state and puts all those data on the screen.
        /// </summary>
        public void Load() 
        {
            DialogueData.Apply(dialogue);
            CharacterData.Apply(characters);
            AudioData.Apply(audio);
            GraphicData.Apply(graphics);
        }
    }
}
