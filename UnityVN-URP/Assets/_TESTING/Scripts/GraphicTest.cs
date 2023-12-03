using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GraphicPanel panel = GraphicPanelController.Instance.GetPanel("Fondo");
        GraphicLayer layer = panel.GetLayer(0, true);

        layer.SetTexture("Graphics/BG Images/casa01");
        layer.CurrentGraphic.renderer.material.SetColor("_Color", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
