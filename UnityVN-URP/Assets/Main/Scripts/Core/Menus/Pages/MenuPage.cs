using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public enum PageType { SaveAndLoad, Config, Help };
    public PageType pageType;

    public virtual void Open()
    {
    }

    public virtual void Close(bool closeAllMenus = false)
    {
        /*
        if (closeAllMenus)
        {
            //VNMenuManager.Instance.CloseRoot();
        }
        */
    }
}
