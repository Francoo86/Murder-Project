using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

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

        if (texture == null)
        {
            Debug.LogError($"Couldn't load the {path} texture from disk.");
            return;
        }

        SetTexture(texture, transitionSpeed, path, blendingTexture);
    }

    // path para poder guardar elementos de Load/Save.
    public void SetTexture(Texture texture, float transitionSpeed = 2.0f, string path = "", Texture blendingTexture = null)
    {
        CreateGraphic(texture, transitionSpeed, path, blendingText: blendingTexture);
    }

    public void SetVideo(string path, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null)
    {
        VideoClip clip = Resources.Load<VideoClip>(path);

        if (clip == null)
        {
            Debug.LogError($"Couldn't load the {path} video clip from disk.");
            return;
        }

        SetVideo(clip, transitionSpeed, useAudio, path, blendingTexture);
    }

    public void SetVideo(VideoClip video, float transitionSpeed = 2.0f, bool useAudio = true, string path = "", Texture blendingTexture = null)
    {
        CreateGraphic(video, transitionSpeed, path, useAudio, blendingTexture);
    }

    // Only way to avoid repeating myself.
    // Usar el tipo generico para poder manejar imagenes y videos.
    private void CreateGraphic<T>(T graphicData, float transitionSpeed, string path, bool useAudioForVids = false, Texture blendingText = null)
    {
        GraphicObject graphObj = null;

        if (graphicData is Texture)
        {
            // Hacer un casting si es una textura.
            graphObj = new GraphicObject(this, path, graphicData as Texture);
        }
        else if (graphicData is VideoClip)
        {
            graphObj = new GraphicObject(this, path, graphicData as VideoClip);
        }

        // Mantener graficas en renderizado (trackeo).
        CurrentGraphic = graphObj;
        CurrentGraphic.FadeIn(transitionSpeed, blendingText);
    }
}
