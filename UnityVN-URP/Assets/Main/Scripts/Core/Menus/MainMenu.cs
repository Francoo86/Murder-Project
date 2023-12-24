using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

public class MainMenu : MonoBehaviour
{
    public AudioClip menuMusic;

    void Start()
    {
        AudioController.Instance.PlayTrack(menuMusic, startVol:1);
    }

    public void StarNewGame()
    {
        VNGameSave.activeFile = new VNGameSave();
        UnityEngine.SceneManagement.SceneManager.LoadScene("VisualNovel");
    }

}
