using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandParameters
{
    private const char PARAM_ID = '-';
    private Dictionary<string, string> parameters = new Dictionary<string, string>();
    /// <summary>
    /// This specifies the params without asignation of them, but with the order.
    /// Like param1 -50, but now only -50.
    /// </summary>
    private List<string> unlabeledParams = new List<string>();

    public CommandParameters(string[] paramArray)
    {
        for(int i = 0; i < paramArray.Length; i++)
        {
            if (paramArray[i].StartsWith(PARAM_ID) && !float.TryParse(paramArray[i], out _))
            {
                string paramName = paramArray[i];
                string pValue = "";

                if(paramArray.Length > i + 1 && !paramArray[i + 1].StartsWith(PARAM_ID)) {
                    pValue = paramArray[i + 1];
                    i++;
                }

                parameters.Add(paramName, pValue);
            }
            else
                unlabeledParams.Add(paramArray[i]);
        }
    }

    public bool TryGetValue<T>(string paramName, out T value, T defaultVal = default(T))
    {
        return TryGetValue(new string[] { paramName }, out value, defaultVal);
    }

    public bool TryGetValue<T>(string[] paramName, out T value, T defaultVal = default(T))
    {
        foreach(string parameter in paramName)
        {
            if(parameters.TryGetValue(parameter, out string paramValue))
            {
                if(TryCastParamater(paramValue, out value)) return true;
            }
        }

        foreach (string parameter in unlabeledParams)
        {
            if (TryCastParamater(parameter, out value))
            {
                unlabeledParams.Remove(parameter);
                return true;
            }
        }

        value = defaultVal;
        return false;
    }

    private bool TryCastParamater<T>(string paramValue, out T value)
    {
        value = default; // Initialize with default value

        if (typeof(T) == typeof(bool))
        {
            if (bool.TryParse(paramValue, out bool boolValue))
            {
                value = (T)(object)boolValue;
                return true;
            }
        }
        else if (typeof(T) == typeof(int))
        {
            if (int.TryParse(paramValue, out int intValue))
            {
                value = (T)(object)intValue;
                return true;
            }
        }
        else if (typeof(T) == typeof(float))
        {
            if (float.TryParse(paramValue, out float floatValue))
            {
                value = (T)(object)floatValue;
                return true;
            }
        }
        else if (typeof(T) == typeof(string))
        {
            value = (T)(object)paramValue;
            return true;
        }

        return false;
    }
}
