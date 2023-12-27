using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class VN_Configuration
{

    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.rootPath}vnconfig.cfg"; //posible error revisar

    public const bool ENCRYPT = false;

    //Ajustes generales
    public bool display_fullscreen = true;
    public string display_resolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;
    //public float dialogueTextSpeed = 1f;
    //public float dialogueAutoReadSpeed = 1f;

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

        /*
        //establecer el tamaño de la pantalla
        int res_index = 0;
        for (int i = 0; i< ui.resolutions.options.Count; i++)
        {
            string resolution = ui.resolutions.options[i].text;
            if (resolution == display_resolution)
            {
                res_index = i;
                break;
            }
        }
        ui.resolutions.value = res_index;*/

        //establecer la opción continuar después de skipping
        //ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);

        //establecer el valor de autoreaderpeed y de architect
        //ui.architectSpeed.value = dialogueTextSpeed;
        //ui.autoReaderSpeed.value = dialogueAutoReadSpeed;

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
