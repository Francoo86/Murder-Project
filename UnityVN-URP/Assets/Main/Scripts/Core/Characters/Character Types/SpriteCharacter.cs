using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCharacter : Character
{
    private const string SPRITE_RENDERED_PARENT_NAME = "Renderers";
    private const string IMAGES_PATH = "/Images";
    public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = false;
    private string assetsDirectory;
    private CanvasGroup RootCanvas => root.GetComponent<CanvasGroup>();
    public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>(); 
    public SpriteCharacter(string name, CharacterConfigData config, GameObject prefab, string charAssetsFolder) : base(name, config, prefab) {
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

        for(int i = 0;  i < renderRoot.transform.childCount; i++)
        {
            Transform child = renderRoot.transform.GetChild(i);
            Image renderImage = child.GetComponent<Image>();

            if(renderImage != null) {
                CharacterSpriteLayer layer = new CharacterSpriteLayer(renderImage);
                layers.Add(layer);
                //En espa�ol porque en la UI de Unity nos pusimos de acuerdo en que fuera as�.
                child.name = $"Capa {i}";
            }
            
        }
    }

    public void SetSprite(Sprite sprite, int layer = 0) {
        layers[layer].SetSprite(sprite);
    }

    public Sprite GetSprite(string spriteName)
    {
        if(config.charType == CharacterType.SpriteSheet)
        {
            return null;
        }
        else
            return Resources.Load<Sprite>($"{assetsDirectory}/{spriteName}");
    }

    public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 0)
    {
        CharacterSpriteLayer spriteLayer = layers[layer];
        return spriteLayer.TransitionSprite(sprite, speed);
    }

    public override IEnumerator HandleShowing(bool shouldShow)
    {
        float targetAlpha = shouldShow ? 1.0f : 0.0f;
        CanvasGroup group = RootCanvas;

        while(group.alpha != targetAlpha)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, 2f * Time.deltaTime);
            yield return null;
        }

        //Resetear las corutinas.
        CO_Hiding = null;
        CO_Showing = null;
    }
}
