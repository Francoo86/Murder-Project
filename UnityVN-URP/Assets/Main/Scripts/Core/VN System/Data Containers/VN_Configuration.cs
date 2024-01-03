using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class VN_Configuration
{
    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.rootPath}vnconfig.cfg"; //posible error revisar

    //Ajustes generales
    public bool display_fullscreen = true;
    public string display_resolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;

    //ajuste de audio
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;

    //otros ajustes
    public float historyLogScale = 1f;

    public void Load()
    {
        var ui = ConfigMenu.instance.ui;

        //AJUSTES GENERALES
        //establecer el tamaño de la ventana
        ConfigMenu.instance.SetDisplayToFullScreen(display_fullscreen);
        ui.SetButtonColors(ui.fullscreen, ui.windowed, display_fullscreen);

        //configurar los volúmenes del mezclador de audio
        ui.musicVolume.value = musicVolume;
        ui.sfxVolume.value = sfxVolume;
        ui.musicMute.sprite = musicMute ? ui.mutedSymbol : ui.unmutedSymbol;
        ui.sfxMute.sprite = sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;

    }

    public void Save()
    {
        FileManager.Save(filePath, JsonUtility.ToJson(this));
    }

}
