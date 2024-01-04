using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using History;

/// <summary>
/// Adapter to some things in android.
/// </summary>
public class AndroidManager : MonoBehaviour
{   
    /// <summary>
    /// Initializes the manager and hides all PC related GUI elements, and shows the phone ones.
    /// </summary>
    private void Start()
    {
        //Make this game for android i guess.
#if !UNITY_ANDROID
            GameObject[] androidObjs = GameObject.FindGameObjectsWithTag("AndroidElement");

            foreach (GameObject go in androidObjs)
            {
                go.SetActive(false);
            }
#else
        GameObject[] windowsElements = GameObject.FindGameObjectsWithTag("WindowsElement");

        foreach (GameObject go in windowsElements)
        {
            go.SetActive(false);
        }
#endif
    }


    private HistoryManager HManager => HistoryManager.Instance;

    /// <summary>
    /// Goes back on the history, like the previous saved state (if any).
    /// </summary>
    public void GoBack()
    {
        HManager.GoBack();
    }

    /// <summary>
    /// Goes forward on the history, only if the last state was saved.
    /// </summary>
    public void GoForward()
    {
        HManager.GoFoward();
    }

    /// <summary>
    /// Opens the history log with the history button.
    /// </summary>
    public void OnHistoryLogOpen()
    {
        var Log = HManager.logManager;
        if (!Log.isOpen)
            Log.Open();
    }

    /// <summary>
    /// Closes the log manager.
    /// </summary>
    public void OnHistoryLogClose()
    {
        var Log = HManager.logManager;
        Log.Close();
    }
}
