using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Principal;
using VISUALNOVEL;

public class SaveAndLoadMenu : MenuPage
{
    
    public static SaveAndLoadMenu Instance { get; private set; }
    private const int MAX_FILES = 6;

    private int currentPage = 1;
    private bool loadedFilesForFirstTime = false;

    public enum MenuFunction { save, load }
    public MenuFunction menuFunction = MenuFunction.save;

    public SaveLoadSlot[] saveSlots;

    private int slotsPerPage => saveSlots.Length;

    public Texture emptyFileImage;

    //Original
    private void Awake() 
    {
        Instance = this;
    }
    
    //Funcion aparentemente buena, comentada para que Unity no webee.
    public override void Open()
    {
        base.Open();
        
        if (!loadedFilesForFirstTime)
            PopulateSaveSlotsForPage(currentPage);
    }

    
    private void PopulateSaveSlotsForPage(int pageNumber)
    {
        currentPage = pageNumber;
        int startingFile = ((currentPage - 1) * slotsPerPage) + 1;
        int endingFile = startingFile + slotsPerPage - 1;

        //int i = 0; 
        //int fileNum = 0;
        for (int i = 0; i < slotsPerPage; i++)
        {
            int fileNum = startingFile + i;
            SaveLoadSlot slot = saveSlots[i];


            if (fileNum < MAX_FILES)
            {
                slot.root.SetActive(true);
                string filePath = $"{FilePaths.gameSaves}{fileNum}{VNGameSave.FILE_TYPE}";
                slot.fileNumber = fileNum;

                slot.SetLoadPath(filePath);

                //slot.filePath = filePath;
                
                slot.PopulateDetails(menuFunction);
            }
            else
            {
                slot.root.SetActive(false);
            }
        }
    }
    
}
