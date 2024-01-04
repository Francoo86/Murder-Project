using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{
    /// <summary>
    /// Class that holds the variables information like the name, value, and the type (in System.Type C# format).
    /// </summary>
    [System.Serializable]
    public class VN_VariableData
    {
        public string name;
        public string value;
        public string type;
    }
}
