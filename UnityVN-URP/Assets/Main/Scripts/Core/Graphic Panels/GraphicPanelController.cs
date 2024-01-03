using UnityEngine;

public class GraphicPanelController : MonoBehaviour
{
    public static GraphicPanelController Instance { get; private set; }
    public const float DEFAULT_TRANSITION_SPEED = 3.0f;
    [field:SerializeField] public GraphicPanel[] allPanels { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    //TODO: Reimplement as hash table.
    //EL PANEL DE LOS BACKGROUNDS ES "Fondo".
    public GraphicPanel GetPanel(string name)
    {
        name = name.ToLower();
        Debug.Log($"All panels count: {allPanels.Length}");

        foreach(var panel in allPanels)
        {
            Debug.Log($"Current panel name: {panel.panelName}");
            if(panel.panelName.ToLower() == name)
                return panel;
        }

        return null;
    }
}
