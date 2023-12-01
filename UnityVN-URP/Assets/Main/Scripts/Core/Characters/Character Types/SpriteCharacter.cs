using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SpriteCharacter : Character
{
    private const string SPRITE_RENDERED_PARENT_NAME = "Renderers";
    private const string IMAGES_PATH = "/Images";
    public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = false;
    private string assetsDirectory = "";
    private CanvasGroup RootCanvas => root.GetComponent<CanvasGroup>();
    public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();
    public SpriteCharacter(string name, CharacterConfigData config, GameObject prefab, string charAssetsFolder) : base(name, config, prefab)
    {
        RootCanvas.alpha = 0;
        assetsDirectory = charAssetsFolder + IMAGES_PATH;

        GetLayers();

        //Show();
        Debug.Log($"Character based on sprites loaded. {name}");
    }

    private void GetLayers()
    {
        Transform renderRoot = animator.transform.Find(SPRITE_RENDERED_PARENT_NAME);

        if (renderRoot == null) return;

        for (int i = 0; i < renderRoot.transform.childCount; i++)
        {
            Transform child = renderRoot.transform.GetChild(i);
            Image renderImage = child.GetComponent<Image>();

            if (renderImage != null)
            {
                CharacterSpriteLayer layer = new CharacterSpriteLayer(renderImage);
                layers.Add(layer);
                //En español porque en la UI de Unity nos pusimos de acuerdo en que fuera así.
                child.name = $"Capa {i}";
            }

        }
    }

    public void SetSprite(Sprite sprite, int layer = 0)
    {
        layers[layer].SetSprite(sprite);
    }

    public Sprite GetSprite(string spriteName)
    {
        if (config.charType == CharacterType.SpriteSheet)
        {
            return null;
        }
        else
            return Resources.Load<Sprite>($"{assetsDirectory}/{spriteName}");
    }

    public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1)
    {
        //TODO: Remove the layer.
        CharacterSpriteLayer spriteLayer = layers[layer];
        return spriteLayer.TransitionSprite(sprite, speed);
    }

    public override IEnumerator HandleShowing(bool shouldShow)
    {
        float targetAlpha = shouldShow ? 1.0f : 0.0f;
        CanvasGroup group = RootCanvas;

        while (group.alpha != targetAlpha)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, 2f * Time.deltaTime);
            yield return null;
        }

        //Resetear las corutinas.
        CO_Hiding = null;
        CO_Showing = null;
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);
        color = displayColor;

        foreach (CharacterSpriteLayer layer in layers)
        {
            layer.StopChangingColor();
            layer.SetColor(color);
        }
    }
    public override IEnumerator ChangingColor(Color color, float speed)
    {
        foreach (CharacterSpriteLayer layer in layers)
            layer.TransitionColor(color, speed);
        yield return null;
        while (layers.Any(l => l.isChangingColor))
            yield return null;
        co_changingColor = null;
    }



    public override IEnumerator Highlighting(bool highlight, float speedMultiplier)
    {
        Color targetColor = displayColor;

        foreach (CharacterSpriteLayer layer in layers)
            layer.TransitionColor(targetColor, speedMultiplier);

        yield return null;

        while (layers.Any(l => l.isChangingColor))
            yield return null;

        co_changingColor = null;
    }

    public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
    {
        foreach (CharacterSpriteLayer layer in layers)
        {
            if (faceLeft)
                layer.FaceLeft(speedMultiplier, immediate);
            else
                layer.FaceRight(speedMultiplier, immediate);
        }
        yield return null;
        while (layers.Any(l => l.isFlipping))
            yield return null;
        co_flipping = null;
    }

    public override void OnExpressionReceive(int layer, string expression)
    {
        Debug.Log($"pls change {layer} {expression}");
        Sprite sprite = GetSprite(expression);

        if (sprite == null) { 
            Debug.LogError($"Expression {expression} not found!!");
            return;
        }
        //base.OnExpressionReceive(layer, expression);

        TransitionSprite(sprite, layer);
    }
}
