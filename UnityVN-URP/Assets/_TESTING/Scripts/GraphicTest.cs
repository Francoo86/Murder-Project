using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTesting());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartTesting()
    {
        GraphicPanel panel = GraphicPanelController.Instance.GetPanel("Fondo");
        GraphicLayer layer = panel.GetLayer(0, true);

        yield return new WaitForSeconds(1);

        Texture blend = Resources.Load<Texture>("Graphics/Transition Effects/blackHole");
        layer.SetTexture("Graphics/BG Images/casa01", blendingTexture: blend);

        yield return new WaitForSeconds(1);

        layer.SetVideo("Graphics/BG Videos/otra vista final", blendingTexture: blend);
        //layer.CurrentGraphic.renderer.material.SetColor("_Color", Color.blue);

        //prueba de desvanecimiento de escena 

        /*
        yield return new WaitForSeconds(3);
        layer.CurrentGraphic.FadeOut();
        yield return new WaitForSeconds(2);
        Debug.Log(layer.CurrentGraphic);
        */

    }

}
