using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public Animator anim;
    private const string OPEN = "Open";
    private const string CLOSE = "Close";
    public enum PageType { SaveAndLoad, Config, Help };
    public PageType pageType;

    public virtual void Open()
    {
        anim.SetTrigger(OPEN);
    }

    public virtual void Close(bool closeAllMenus = false)
    {
        anim.SetTrigger(CLOSE);
        if (closeAllMenus)
        {
            VNMenuManager.Instance.CloseRoot();
        }
    }
}
