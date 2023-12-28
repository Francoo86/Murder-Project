using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using VISUALNOVEL;
    
public class SaveLoadSlot : MonoBehaviour
{
    
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI titleText;
    public Button deleteButton;
    public Button loadButton;
    public Button saveButton;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath = "";

    
    public void PopulateDetails( SaveAndLoadMenu.MenuFunction function )
    {
        if (File.Exists(filePath))
        {
            //UnityEngine.Debug.Log($"<color=#AAFF00>MEGA SCREAMING AT CHECKING INSTANCE THINGS PART 1: {VNGameSave.activeFile.playerName}</color>");
            VNGameSave file = VNGameSave.Load(filePath);
            //Debug.Log($"<color=#AAFF00>MEGA SCREAMING AT CHECKING INSTANCE THINGS PART 2: {VNGameSave.activeFile.playerName}</color>");
            PopulateDetailsFromFile(function, file);
        }
        else 
        {
            PopulateDetailsFromFile(function, null);
        }
    }
    
    
    private void PopulateDetailsFromFile( SaveAndLoadMenu.MenuFunction function, VNGameSave file) 
    {
        //Debug.Log("IS THIS SAVE BEING CALLED?!!?!?!");
        //Debug.Log($"Current file {file}");
        //Debug.Log($"Actual instance of the file {SaveAndLoadMenu.Instance.emptyFileImage}");
        if (file == null)
        {
            titleText.text = $"{fileNumber}. Empty File";
            deleteButton.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.save);
            //Este es el conchesumare que webea.
            previewImage.texture = SaveAndLoadMenu.Instance.emptyFileImage;
        }
        else
        {
            titleText.text = $"{fileNumber}. {file.timestamp}";
            deleteButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.load);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.save);

            byte[] data = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, data);
            previewImage.texture = screenshotPreview;
        }
    }
    
    
    public void Delete() 
    {
        File.Delete(filePath);
        PopulateDetails(SaveAndLoadMenu.Instance.menuFunction);
    }

    public void Load()
    {
        VNGameSave file = VNGameSave.Load(filePath, false);
        SaveAndLoadMenu.Instance.Close(closeAllMenus: true);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == MainMenu.MAIN_MENU_SCENE)
        {
            //VNGameSave.activeFile = file;
            MainMenu.instance.LoadGame(file);
        }
        else
        {
            //Debug.Log($"Currently loading file number of: {filePath}");
            file.Activate();
            VNGameSave.activeFile = file;
        }

    }
    
    public void Save()
    {

        var activeSave = VNGameSave.activeFile;
        //Debug.Log($"Currently saving file number of: {fileNumber}");
        activeSave.slotNumber = fileNumber;
        activeSave.Save();
        PopulateDetailsFromFile(SaveAndLoadMenu.Instance.menuFunction, activeSave);
    }
    
}
