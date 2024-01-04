using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

namespace VISUALNOVEL
{
    /// <summary>
    /// Saves the conversation by the file name, and saves by starting and ending of the conversation also the progress.
    /// </summary>
    [System.Serializable]
    public class VN_ConversationDataCompressed
    {
        public string fileName;
        public int startIndex;
        public int endIndex;
        public int progress;
    }
}