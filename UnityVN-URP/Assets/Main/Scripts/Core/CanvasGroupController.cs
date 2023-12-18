using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manipulates the CanvasGroup of a MonoBehaviour object.
/// </summary>
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

    public float Alpha { get { return canvas.alpha; } set { canvas.alpha = value; } }

    /// <summary>
    /// Initializes the CanvasGroupController with an owner and its CanvasGroup.
    /// </summary>
    /// <param name="owner">The MonoBehaviour owner.</param>
    /// <param name="canvas">The associated CanvasGroup from the owner.</param>

    public CanvasGroupController(MonoBehaviour owner, CanvasGroup canvas)
    {
        this.owner = owner;
        this.canvas = canvas;
    }

    /// <summary>
    /// Shows an element in the screen.
    /// </summary>
    /// <param name="speed">How fast will be the show transition.</param>
    /// <param name="inmediate">Show inmediatly.</param>
    /// <returns>Coroutine to be used by the owner.</returns>

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

    /// <summary>
    /// Hides the element in the screen.
    /// </summary>
    /// <param name="speed">How fast should be hide.</param>
    /// <param name="inmediate">Hide inmediatly.</param>
    /// <returns>Coroutine to be used by the owner.</returns>
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

    
    /// <summary>
    /// Fades in or fades out the owner.
    /// </summary>
    /// <param name="alpha">The target alpha (from 0 to 1)</param>
    /// <param name="speed">How fast should be the transition.</param>
    /// <param name="inmediate">Fades inmediatly without any transition.</param>
    /// <returns>IEnumerator yield.</returns>
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

    /// <summary>
    /// Makes the owner interactable with the screen or be able to receive player input.
    /// </summary>
    /// <param name="active">Should be active?</param>
    public void SetInteractableState(bool active)
    {
        canvas.blocksRaycasts = active;
        canvas.interactable = active;
    }

}
