using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//MenuPage.PageType

public class VNMenuManager : MonoBehaviour
{
    //Filepath por lo que veo, webea mucho, de todas formas me imagino que nosotros debemos asignarlas.
    public static VNMenuManager Instance;
    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;
    private CanvasGroupController rootCG;

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

    // Dan probelmas el slm.menuFunction -> se debe crear en VNGAMESAVE.
    public void OpenSavePage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.save;
        OpenPage(page);
    }

    public void OpenLoadPage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.load;
        OpenPage(page);
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

    public void OpenPage(MenuPage page) 
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
    
}
