using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicLayer
{
    public const string LAYER_NAME_FORMAT = "Layer {0}";
    public Transform panel;
    public int layerDepth = 0;
    public GraphicObject CurrentGraphic { get; private set; } = null;
    // Start is called before the first frame update
    public void SetTexture(string path, float transitionSpeed = 2.0f, Texture blendingTexture = null)
    {
        Texture texture = Resources.Load<Texture2D>(path);

        if(texture == null)
        {
            Debug.LogError($"Couldn't load the {path} texture from disk.");
            return;
        }

        SetTexture(texture, transitionSpeed, blendingTexture, path);
    }
    
    //path para poder guardar elementos de Load/Save.
    public void SetTexture(Texture texture, float transitionSpeed = 2.0f, Texture blendingTexture = null, string path = "")
    {
        CreateGraphic(texture, transitionSpeed, path, blendingTexture);
    }
    
    //Only way to avoid repeating myself.
    //Usar el tipo generico para poder manejar imagenes y videos.
    private void CreateGraphic<T>(T graphicData, float transitionSpeed, string path, bool useAudioForVids = false, Texture blendingText = null)
    {
        GraphicObject graphObj = null;

        if (graphicData is Texture)
            //Hacer un casteo si es una textura.
            graphObj = new GraphicObject(this, path, graphicData as Texture);

        //Mantener graficas en renderizado (trackeo).
        CurrentGraphic = graphObj;
    }
}
