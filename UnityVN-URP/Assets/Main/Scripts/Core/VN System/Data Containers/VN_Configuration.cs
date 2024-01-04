using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The configuration object of the Visual Novel.
/// </summary>
[System.Serializable]
public class VN_Configuration
{
    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.rootPath}vnconfig.cfg";
    public bool display_fullscreen = true;
    public bool continueSkippingAfterChoice = false;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;

    /// <summary>
    /// Loads the configuration, in this case the fullscreen mode and sounds related stuff.
    /// </summary>
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

    /// <summary>
    /// Saves the configuration into JSON.
    /// </summary>
    public void Save()
    {
        FileManager.Save(filePath, JsonUtility.ToJson(this));
    }

}
