using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GraphicPanelController.Instance.GetPanel("Fondo").GetLayer(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
