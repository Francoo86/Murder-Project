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
        //There could be other ways to do this.
        GameObject closeBtn = GameObject.Find("CloseHistoryAndroid");
        GameObject historyBtn = GameObject.Find("HistoryButtonAndroid");

        //Make this game for android i guess.
        #if !UNITY_ANDROID
              closeBtn.SetActive(false);
              historyBtn.SetActive(false);
        #endif
    }


    HistoryLogManager Log => HistoryManager.Instance.logManager;

    public void OnHistoryLogOpen()
    {
        if (!Log.isOpen)
            Log.Open();
    }

    public void OnHistoryLogClose()
    {
        Log.Close();
    }
}
