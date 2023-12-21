using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// The type of characters that use a sprite to interact.
/// </summary>
public class SpriteCharacter : Character
{
    private const string SPRITE_RENDERED_PARENT_NAME = "Renderers";
    private const string IMAGES_PATH = "/Images";
    public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = false;
    private string assetsDirectory = "";
    private CanvasGroup RootCanvas => root.GetComponent<CanvasGroup>();

    public override bool IsVisible => IsShowing || RootCanvas.alpha == 1;

    //TODO: Esto hay que cambiarlo por uno solo.
    public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();
    /// <summary>
    /// Creates a SpriteCharacter associated with the configuration specified in the Unity file and the prefab defined with it.
    /// </summary>
    /// <param name="name">Character name.</param>
    /// <param name="config">The configuration data of the character.</param>
    /// <param name="prefab">The prefab (images) of the character.</param>
    /// <param name="charAssetsFolder">The folder that stores the character images.</param>
    public SpriteCharacter(string name, CharacterConfigData config, GameObject prefab, string charAssetsFolder) : base(name, config, prefab)
    {
        RootCanvas.alpha = 0;
        assetsDirectory = charAssetsFolder + IMAGES_PATH;

        GetLayers();

        //Show();
        Debug.Log($"Character based on sprites loaded. {name}");
    }

    //FIXME: Remove the layers thing, we only use one.
    /// <summary>
    /// Gets the layers of a character, currently this function will be modified because we only use 1 layer.
    /// </summary>
    private void GetLayers()
    {
        Transform renderRoot = animator.transform.Find(SPRITE_RENDERED_PARENT_NAME);

        if (renderRoot == null) return;

        //TODO: Set it only to first child as we don't use all the layers.
        for (int i = 0; i < renderRoot.transform.childCount; i++)
        {
            Transform child = renderRoot.transform.GetChild(i);
            
            if (child.TryGetComponent<Image>(out var renderImage))
            {
                CharacterSpriteLayer layer = new CharacterSpriteLayer(renderImage);
                layers.Add(layer);
                //En español porque en la UI de Unity nos pusimos de acuerdo en que fuera así.
                //FIXME: Quitar esto.
                child.name = $"Capa {i}";
            }

        }
    }

    /// <summary>
    /// Sets the sprite of the character and the layer to be saved.
    /// </summary>
    /// <param name="sprite">The sprite resource.</param>
    /// <param name="layer">The layer where it lies.</param>
    public void SetSprite(Sprite sprite, int layer = 0)
    {
        layers[layer].SetSprite(sprite);
    }

    /// <summary>
    /// Gets the sprite based on the expression name of the character.
    /// </summary>
    /// <param name="spriteName">The expression name of the character.</param>
    /// <returns>The sprite associated with the expression.</returns>
    public Sprite GetSprite(string spriteName)
    {
        return Resources.Load<Sprite>($"{assetsDirectory}/{spriteName}");
    }


    /// <summary>
    /// Makes a transition while changing the character sprite.
    /// </summary>
    /// <param name="sprite">The sprite resource.</param>
    /// <param name="layer">The layer to change the sprite.</param>
    /// <param name="speed">How fast the transition should be.</param>
    /// <returns>The Coroutine process associated with the transition.</returns>
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



    public override IEnumerator Highlighting(bool highlight, float speedMultiplier, bool inmediate = false)
    {
        Color targetColor = displayColor;

        foreach (CharacterSpriteLayer layer in layers)
        {
            if (inmediate)
                layer.SetColor(targetColor);
            else
                layer.TransitionColor(targetColor, speedMultiplier);
        }
            

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
        Sprite sprite = GetSprite(expression);

        if (sprite == null) { 
            Debug.LogError($"Expression {expression} not found!!");
            return;
        }

        TransitionSprite(sprite, layer);
    }
}
