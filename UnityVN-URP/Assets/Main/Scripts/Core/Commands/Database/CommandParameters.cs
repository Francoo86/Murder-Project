using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandParameters
{
    private const char PARAM_ID = '-';
    private Dictionary<string, string> parameters = new Dictionary<string, string>();

    public CommandParameters(string[] paramArray)
    {
        for(int i = 0; i < paramArray.Length; i++)
        {
            if (paramArray[i].StartsWith(PARAM_ID))
            {
                string paramName = paramArray[i];
                string pValue = "";

                if(paramArray.Length > i + 1 && !paramArray[i + 1].StartsWith(PARAM_ID)) {
                    pValue = paramArray[i + 1];
                    i++;
                }

                parameters.Add(paramName, pValue);
            }


        }
    }
}
