using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;


public class ConfigMenu : MenuPage
{
    public static ConfigMenu instance { get; private set; }

    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;

    public UI_ITEMS ui;
    private VN_Configuration config => VN_Configuration.activeConfig;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];

        //Resolution[] resolutions = Screen.resolutions; // esta linea aparentemente en el video desaparece

       // SetAvailableResolutions();

        LoadConfig();
    }

    private void LoadConfig()
    {
        if (File.Exists(VN_Configuration.filePath))
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath);
        else
            VN_Configuration.activeConfig = new VN_Configuration();

        VN_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        VN_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());
        if (panel == null)
        {
            UnityEngine.Debug.LogWarning($"Did not find panel called '{panelName}' in config menu");
            return;
        }

        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
    }

    [System.Serializable]
    public class UI_ITEMS
    {
        private static Color button_selectedColor = new Color(1, 0.35f, 0, 1);
        private static Color button_unselectedColor = new Color(1f, 1f, 1f, 1);
        private static Color text_selectedColor = new Color(1, 1f, 0, 1);
        private static Color text_unselectedColor = new Color(0.25f, 0.25f, 0.25f, 1);
        public static Color musicOnColor = new Color(1, 0.65f, 0, 1);
        public static Color musicOffColor = new Color(0.5f, 0.5f, 0.5f, 1);

        [Header("General")]
        public Button fullscreen;
        public Button windowed;

        [Header("Audio")]
        public Slider musicVolume;
        public Image musicFill;
        public Slider sfxVolume;
        public Image sfxFill;
        public Sprite mutedSymbol;
        public Sprite unmutedSymbol;
        public Image musicMute;
        public Image sfxMute;

        public void SetButtonColors(Button A, Button B, bool selectedA)
        {
            A.GetComponent<Image>().color = selectedA ? button_selectedColor : button_unselectedColor;
            B.GetComponent<Image>().color = !selectedA ? button_selectedColor : button_unselectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? text_selectedColor : text_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = !selectedA ? text_selectedColor : text_unselectedColor;
        }

    }

    //funciones invocables de la UI
    public void SetDisplayToFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetMusicVolume()
    {
        Debug.Log($"Music volume is: {ui.musicVolume.value}");
        config.musicVolume = ui.musicVolume.value;
        AudioController.Instance.SetMusicVolume(config.musicVolume, config.musicMute);

        ui.musicFill.color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetSFXVolume()
    {
        config.sfxVolume = ui.sfxVolume.value;
        AudioController.Instance.SetSFXVolume(config.sfxVolume, config.sfxMute);

        ui.musicFill.color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetMusicMute()
    {
        config.musicMute = !config.musicMute;
        ui.musicVolume.fillRect.GetComponent<Image>().color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.musicMute.sprite = config.musicMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioController.Instance.SetMusicVolume(config.musicVolume, config.musicMute);
    }

    public void SetSFXMute()
    {
        config.sfxMute = !config.sfxMute;
        ui.sfxVolume.fillRect.GetComponent<Image>().color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioController.Instance.SetSFXVolume(config.sfxVolume, config.sfxMute);
    }

}
