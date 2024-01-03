using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

public class MainMenu : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "Main Menu";

    public static MainMenu instance { get; private set; }

    public AudioClip menuMusic;
    public CanvasGroup mainPanel;
    public CanvasGroupController mainCG;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainCG = new CanvasGroupController(this, mainPanel);
        AudioController.Instance.PlayTrack(menuMusic, channel: 0, startVol : 1);
    }

    public void LoadGame(VNGameSave file)
    {
        VNGameSave.activeFile = file;
        StartCoroutine(StartingGame());
    }

    public void StarNewGame()
    {
        VNGameSave.activeFile = new VNGameSave();
        StartCoroutine(StartingGame());
    }

    private IEnumerator StartingGame()
    {
        mainCG.Hide(speed: 0.3f);
        AudioController.Instance.StopTrack(0);

        while (mainCG.IsVisible)
            yield return null;

        UnityEngine.SceneManagement.SceneManager.LoadScene("VisualNovel");
    }
}
