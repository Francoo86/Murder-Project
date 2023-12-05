using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;


public class GraphicObject
{
    private const string NAME_FORMAT = "Grafica - [{0}]";
    private const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
    private const string MAT_FIELD_COLOR = "_Color";
    private const string MAT_FIELD_MAINTEX = "_MainTex";
    private const string MAT_FIELD_BLENDTEX = "_BlendTex";
    private const string MAT_FIELD_BLEND = "_Blend";
    private const string MAT_FIELD_ALPHA = "_Alpha";

    private Coroutine co_FadingIn = null;
    private Coroutine co_FadingOut = null;

    public RawImage renderer;
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

        // Nombre de la imagen en la escena.
        renderer.name = string.Format(NAME_FORMAT, GraphicName);
        renderer.material.SetTexture(MAT_FIELD_MAINTEX, texture);
    }

    public GraphicObject(GraphicLayer layer, string graphicPath, VideoClip clip)
    {
        this.graphicPath = graphicPath;

        GameObject obj = new GameObject();
        obj.transform.SetParent(layer.panel);
        renderer = obj.AddComponent<RawImage>();

        GraphicName = clip.name;

        InitializeGraphics();

        // Renderizar video
        RenderTexture texture = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
        renderer.material.SetTexture(MAT_FIELD_MAINTEX, texture);

        vidPlayer = renderer.gameObject.AddComponent<VideoPlayer>();
        vidPlayer.playOnAwake = true;
        vidPlayer.source = VideoSource.VideoClip;
        vidPlayer.clip = clip;
        vidPlayer.renderMode = VideoRenderMode.RenderTexture;
        vidPlayer.targetTexture = texture;
        vidPlayer.isLooping = true;

        vidPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audioSource = renderer.gameObject.AddComponent<AudioSource>();

        audioSource.volume = 0;

        vidPlayer.SetTargetAudioSource(0, audioSource);

        vidPlayer.frame = 0;
        vidPlayer.Prepare();
        vidPlayer.Play();

        vidPlayer.enabled = false;
        vidPlayer.enabled = true;
    }

    private void InitializeGraphics()
    {
        // Inicializar en la posición del panel.
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect = renderer.GetComponent<RectTransform>();
        rect.anchorMin = Vector3.zero;
        rect.anchorMax = Vector3.one;

        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.one;

        // Cargar las transiciones.
        renderer.material = GetTransitionMaterial();
        renderer.material.SetFloat(MAT_FIELD_BLEND, 0);
        renderer.material.SetFloat(MAT_FIELD_ALPHA, 0);
    }

    private Material GetTransitionMaterial()
    {
        Material mat = Resources.Load<Material>(MATERIAL_PATH);

        if (mat != null)
        {
            return new Material(mat);
        }

        return null;
    }

    GraphicPanelController GraphicController => GraphicPanelController.Instance;

    public Coroutine FadeIn(float speed, Texture blend = null)
    {
        if (co_FadingOut != null)
            GraphicController.StopCoroutine(co_FadingOut);

        if (co_FadingIn != null)
            return co_FadingIn;

        co_FadingIn = GraphicController.StartCoroutine(Fading(1f, speed, blend));

        return co_FadingIn;
    }

    public Coroutine FadeOut(float speed, Texture blend = null)
    {
        if (co_FadingIn != null)
            GraphicController.StopCoroutine(co_FadingIn);

        if (co_FadingOut != null)
            return co_FadingOut;

        co_FadingOut = GraphicController.StartCoroutine(Fading(0f, speed, blend));

        return co_FadingOut;
    }

    private IEnumerator Fading(float target, float speed, Texture blend = null)
    {
        bool isBlending = blend != null;
        bool fadingIn = target > 0f;

        Material renderMat = renderer.material;
        renderMat.SetTexture(MAT_FIELD_BLENDTEX, blend);
        renderMat.SetFloat(MAT_FIELD_ALPHA, isBlending ? 1f : fadingIn ? 0f : 1f);
        renderMat.SetFloat(MAT_FIELD_BLEND, isBlending ? fadingIn ? 0 : 1 : 1);

        string opacityParam = isBlending ? MAT_FIELD_BLEND : MAT_FIELD_ALPHA;

        while (renderMat.GetFloat(opacityParam) != target)
        {
            float opacity = Mathf.MoveTowards(renderMat.GetFloat(opacityParam), target, speed * Time.deltaTime);
            renderMat.SetFloat(opacityParam, opacity);

            if (IsVideo)
                audioSource.volume = opacity;

            yield return null;
        }

        co_FadingIn = null;
        co_FadingOut = null;
    }
}
