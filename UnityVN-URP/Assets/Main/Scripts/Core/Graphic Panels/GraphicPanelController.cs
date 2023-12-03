using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelController : MonoBehaviour
{
    public static GraphicPanelController Instance { get; private set; }
    public const float DEFAULT_TRANSITION_SPEED = 3.0f;
    [SerializeField] public GraphicPanel[] allPanels;

    void Awake()
    {
        Instance = this;
    }

    //TODO: Reimplement as hash table.
    public GraphicPanel GetPanel(string name)
    {
        name = name.ToLower();

        foreach(var panel in allPanels)
        {
            if(panel.panelName.ToLower() == name)
                return panel;
        }

        return null;
    }
}
