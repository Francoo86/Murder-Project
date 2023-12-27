using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using History;

/// <summary>
/// Adapter to some things in android.
/// </summary>
public class AndroidManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    private void Start()
    {
        //Make this game for android i guess.
        #if !UNITY_ANDROID
            GameObject[] androidObjs = GameObject.FindGameObjectsWithTag("AndroidElement");

            foreach (GameObject go in androidObjs)
            {
                go.SetActive(false);
            }
        #endif
    }


    private HistoryManager HManager => HistoryManager.Instance;

    public void GoBack()
    {
        HManager.GoBack();
    }

    public void GoForward()
    {
        HManager.GoFoward();
    }

    public void OnHistoryLogOpen()
    {
        var Log = HManager.logManager;
        if (!Log.isOpen)
            Log.Open();
    }

    public void OnHistoryLogClose()
    {
        var Log = HManager.logManager;
        Log.Close();
    }
}
