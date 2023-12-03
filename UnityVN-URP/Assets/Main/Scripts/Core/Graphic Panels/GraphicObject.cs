using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GraphicObject : MonoBehaviour
{
    private const string NAME_FORMAT = "Grafica - [{0}]";
    //Casi dejo la embarrá con esto XDDDD
    private const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
    public RawImage renderer;
    //Para el juicio final me imagino.

    public VideoPlayer vidPlayer = null;
    public AudioSource audioSource = null;
    public string graphicPath = "";
    public bool IsVideo { get { return vidPlayer != null; } }
    public string GraphicName { get; private set; }

    public GraphicObject(GraphicLayer layer, string graphicPath, Texture texture)
    {
        this.graphicPath = graphicPath;

        GameObject obj = new GameObject();
        obj.transform.SetParent(layer.panel);
        renderer = obj.AddComponent<RawImage>();

        GraphicName = texture.name;

        InitializeGraphics();

        //Nombre de la imagen en la escena.
        renderer.name = string.Format(NAME_FORMAT, GraphicName);
        //Textura de la imagen.
        renderer.texture = texture;

        //Cargar las transiciones.
        renderer.material = GetTransitionMaterial();
    }

    private void InitializeGraphics() { 
        //Inicializar en la posicion del panel.
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect = renderer.GetComponent<RectTransform>();
        rect.anchorMin = Vector3.zero;
        rect.anchorMax = Vector3.one;

        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.one;
    }

    private Material GetTransitionMaterial()
    {
        Material mat = Resources.Load<Material>(MATERIAL_PATH);

        if(mat != null)
        {
            return new Material(mat);
        }

        return null;
    }
}
