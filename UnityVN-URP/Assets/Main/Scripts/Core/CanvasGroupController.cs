using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupController
{
    private MonoBehaviour owner;
    private CanvasGroup canvas;

    private const float DEFAULT_FADE_SPEED = 2.5f;

    //Coroutinas para mostrar y ocultar texto.
    private Coroutine co_Hiding = null;
    private Coroutine co_Showing = null;

    //Checks importantes.
    public bool IsHiding => co_Hiding != null;
    public bool IsShowing => co_Showing != null;
    public bool IsFading => IsHiding || IsShowing;
    public bool IsVisible => IsShowing || canvas.alpha > 0;

    public CanvasGroupController(MonoBehaviour owner, CanvasGroup canvas)
    {
        this.owner = owner;
        this.canvas = canvas;
    }

    public Coroutine Show(float speed = 1, bool inmediate = false)
    {
        if (IsShowing) return co_Showing;
        else if (IsHiding)
        {
            DialogController.Instance.StopCoroutine(co_Hiding);
            co_Hiding = null;
        }

        co_Showing = DialogController.Instance.StartCoroutine(Fade(1, speed, inmediate));

        return co_Showing;
    }

    public Coroutine Hide(float speed = 1, bool inmediate = false)
    {
        if (IsHiding) return co_Hiding;
        else if (IsShowing)
        {
            owner.StopCoroutine(co_Showing);
            co_Showing = null;
        }

        co_Hiding = owner.StartCoroutine(Fade(0, speed, inmediate));
        return co_Hiding;
    }

    //Alpha hacia 1 signfica que se va a mostrar. Para 0 es lo contrario, se esconderá.
    //Estos tipos de metodos son mucha paja xdddd
    private IEnumerator Fade(float alpha, float speed = 1, bool inmediate = false)
    {
        CanvasGroup cg = canvas;

        if (inmediate)
            cg.alpha = alpha;

        while (cg.alpha != alpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED * speed);
            yield return null;
        }

        co_Hiding = null;
        co_Showing = null;
    }
}
