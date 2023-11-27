using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//MenuPage.PageType

public class VNMenuManager : MonoBehaviour
{
    
    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;
    
    private CanvasGroupController rootCG;

    // Start is called before the first frame update
    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
    }

    private MenuPage GetPage(MenuPage.PageType pagetype)
    {
        return pages.FirstOrDefault(page => page.pageType == pagetype);
    }

    public void OpenSavePage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        OpenPage(page);
    }

    public void OpenLoadPage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
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
        isOpen = true;
    }

    public void CloseRoot()
    {
        rootCG.Hide();
        isOpen = false;
    }

}
