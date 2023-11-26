using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCharacter : Character
{
    private CanvasGroup RootCanvas => root.GetComponent<CanvasGroup>();
    public SpriteCharacter(string name, CharacterConfigData config, GameObject prefab) : base(name, config, prefab) {
        RootCanvas.alpha = 0;
        Show();
        Debug.Log($"Character based on sprites loaded. {name}");
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
