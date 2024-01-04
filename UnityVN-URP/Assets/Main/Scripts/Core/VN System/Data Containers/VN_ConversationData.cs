using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

namespace VISUALNOVEL
{
    /// <summary>
    /// The conversation data fully raw and saves the progress.
    /// </summary>
    [System.Serializable]
    public class VN_ConversationData
    {
        public List<string> conversation = new List<string> ();
        public int progress;
    }
    
}
