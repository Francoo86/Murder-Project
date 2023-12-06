using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GraphicLayer
{
    public const string LAYER_NAME_FORMAT = "Layer {0}";
    public Transform panel;
    public int layerDepth = 0;
    public GraphicObject CurrentGraphic = null;
    private List<GraphicObject> oldGraphics = new List<GraphicObject>();

    // Start is called before the first frame update
    public Coroutine SetTexture(string path, float transitionSpeed = 2.0f, Texture blendingTexture = null, bool immediate = false)
    {
        Texture texture = Resources.Load<Texture2D>(path);

        if (texture == null)
        {
            Debug.LogError($"Couldn't load the {path} texture from disk.");
            return null;
        }

        return SetTexture(texture, transitionSpeed, blendingTexture, path, immediate);
    }

    // path para poder guardar elementos de Load/Save.
    public Coroutine SetTexture(Texture texture, float transitionSpeed = 2.0f, Texture blendingTexture = null, string path = "", bool immediate = false)
    {
        return CreateGraphic(texture, transitionSpeed, path, blendingTexture: blendingTexture, immediate: immediate);
    }

    public Coroutine SetVideo(string path, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null, bool immediate = false)
    {
        VideoClip clip = Resources.Load<VideoClip>(path);

        if (clip == null)
        {
            Debug.LogError($"Couldn't load the {path} video clip from disk.");
            return null;
        }

        return SetVideo(clip, transitionSpeed, useAudio, blendingTexture, path, immediate);
    }

    public Coroutine SetVideo(VideoClip video, float transitionSpeed = 2.0f, bool useAudio = true, Texture blendingTexture = null, string path = "", bool immediate = false)
    {
        return CreateGraphic(video, transitionSpeed, path, useAudio, blendingTexture, immediate);
    }

    // Only way to avoid repeating myself.
    // Usar el tipo generico para poder manejar imagenes y videos.
    private Coroutine CreateGraphic<T>(T graphicData, float transitionSpeed, string path, bool useAudioForVids = false, Texture blendingTexture = null, bool immediate = false)
    {
        GraphicObject graphObj = null;

        if (graphicData is Texture)
        {
            // Hacer un casting si es una textura.
            graphObj = new GraphicObject(this, path, graphicData as Texture, immediate);
        }
        else if (graphicData is VideoClip)
        {
            graphObj = new GraphicObject(this, path, graphicData as VideoClip, useAudioForVids, immediate);
        }

        if (CurrentGraphic != null && !oldGraphics.Contains(CurrentGraphic))
            oldGraphics.Add(CurrentGraphic);

        // Mantener graficas en renderizado (trackeo).
        CurrentGraphic = graphObj;
        if (!immediate)
            return CurrentGraphic.FadeIn(transitionSpeed, blendingTexture);
        DestroyOldGraphics();
        return null;
    }

    public void DestroyOldGraphics()
    {
        foreach (var g in oldGraphics)
            Object.Destroy(g.renderer.gameObject);

        oldGraphics.Clear();
    }

    public void Clear()
    {
        if (CurrentGraphic != null)
            CurrentGraphic.FadeOut();

        foreach (var g in oldGraphics)
            g.FadeOut();
    }
}
