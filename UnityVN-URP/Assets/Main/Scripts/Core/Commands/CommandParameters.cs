using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Command parameter handler that tries to parse values like (numbers, bool, strings) associated to a param like -param1 "someparam".
/// </summary>
public class CommandParameters
{
    private const char PARAM_ID = '-';
    private Dictionary<string, string> parameters = new Dictionary<string, string>();
    /// <summary>
    /// This specifies the params without asignation of them, but with the order.
    /// Like param1 -50, but now only -50.
    /// </summary>
    private List<string> unlabeledParams = new List<string>();

    /// <summary>
    /// Creates the object and stores the parameters info.
    /// </summary>
    /// <param name="paramArray">The parameters to be parsed.</param>
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

    /// <summary>
    ///  Tries to get a value associated with a paramater and cast it. Overloaded method with the same name but now for strings only (not arrays).
    /// </summary>
    /// <typeparam name="T">The base type of the variable (float, bool, string, int).</typeparam>
    /// <param name="paramName">The string parameter like "-param1"</param>
    /// <param name="value">The value where it should be out (same type of the base).</param>
    /// <param name="defaultVal">Default value it can't be retrieved.</param>
    /// <returns></returns>
    public bool TryGetValue<T>(string paramName, out T value, T defaultVal = default(T))
    {
        return TryGetValue(new string[] { paramName }, out value, defaultVal);
    }

    /// <summary>
    /// Tries to get a value associated with a paramater and cast it.
    /// </summary>
    /// <typeparam name="T">The base type of the variable (float, bool, string, int).</typeparam>
    /// <param name="paramName">The params defined like {"-param1, "-param2", "-param3"}</param>
    /// <param name="value">The final value to be casted.</param>
    /// <param name="defaultVal">Default value if it can't be parsed.</param>
    /// <returns>Wether the value could be retrieved or not.</returns>
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

    /// <summary>
    /// Tries to cast the parameter by the supported types.
    /// </summary>
    /// <typeparam name="T">The base type like (float, bool, string, int).</typeparam>
    /// <param name="paramValue">The value in string form.</param>
    /// <param name="value">The final variable to be casted (in the same types defined above).</param>
    /// <returns>Wether the variable could be casted or not.</returns>
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
