using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class ConfigMenu : MenuPage
{
    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        for (i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }
        activePanel = panels[0];
        //Resolution[] resolutions = Screen.resolutions;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.FirstOrDefault(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"Did not find panel called '{panelName}' in config menu.");
            return;
        }

        panel.SetActive(true);

        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        //panel.SetActive(true);
        //activePanel = panel;
    }
}
