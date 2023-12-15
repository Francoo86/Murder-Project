using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public enum PageType { SaveAndLoad, Config, Help };
    public PageType pageType;

    public void Open()
    {
    }

    public void Close(bool closeAllMenus = false)
    {
        if (closeAllMenus)
        {
            VNMenuManager.Instance.CloseRoot();
        }
    }
}
