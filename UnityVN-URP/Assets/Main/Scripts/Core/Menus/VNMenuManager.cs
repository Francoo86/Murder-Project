using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VISUALNOVEL;
using UnityEngine.SceneManagement;

//MenuPage.PageType

/// <summary>
/// Class that handles the main menu and gameplay menu logic, providing handling of Saving/Loading/Configuration.
/// </summary>
public class VNMenuManager : MonoBehaviour
{
    public static VNMenuManager Instance;
    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;
    private CanvasGroupController rootCG;

    /// <summary>
    /// Saves the instance to be used across the game.
    /// </summary>
    private void Awake() 
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
    }

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    /// <summary>
    /// Opens the save and load menu related to a specific criteria (load or save).
    /// </summary>
    /// <param name="menuFunc"></param>
    private void OpenPageByCriteria(SaveAndLoadMenu.MenuFunction menuFunc)
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = menuFunc;
        OpenPage(page);
    }

    /// <summary>
    /// Opens the save page and tries to retrieve all the available files related to savings.
    /// </summary>
    public void OpenSavePage()
    {
        OpenPageByCriteria(SaveAndLoadMenu.MenuFunction.save);
    }

    /// <summary>
    /// Opens the load page and tries to retrieve all the available files related to savings.
    /// </summary>
    public void OpenLoadPage()
    {
        OpenPageByCriteria(SaveAndLoadMenu.MenuFunction.load);
    }
    

    public void OpenConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    /// <summary>
    /// Opens a page associated with a type (they need to attached in Unity).
    /// In this case we have the Help menu, that shows the controls.
    /// SaveAndLoad that shows the save slots menu.
    /// Config menu, as the name says it shows the configuration.
    /// </summary>
    /// <param name="page"></param>
    private void OpenPage(MenuPage page) 
    {
        if (page == null)
        {
            return;
        }
        if (activePage != null && activePage != page)
        {
            activePage.Close();
        }

        page.Open();

        activePage = page;

        if(!isOpen)
            OpenRoot();
    }

    public void OpenRoot() 
    {
        rootCG.Show();
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        rootCG.Hide();
        rootCG.SetInteractableState(false);
        isOpen = false;
    }
    
    public void Click_Home()
    {
        VN_Configuration.activeConfig.Save();
        SceneManager.LoadScene(MainMenu.MAIN_MENU_SCENE);
    }

    public void Click_Quit()
    {
        Application.Quit();
    }

}
