using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Character
{
    
    private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f;

    public string name;
    public string displayName;
    //Hacer un cuadro para cada imagen de personaje.
    public RectTransform root;
    public CharacterConfigData config;
    public Animator animator;

    public Color color { get; protected set; } = Color.white;
    protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;

    protected CharacterController controller => CharacterController.Instance;

    
    protected Color highlightedColor => color;
    protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);
    public bool highlighted { get; private set; } = true;


    //Corutinas de mostrado.                              
    protected Coroutine CO_Hiding, CO_Showing, CO_Moving, co_highlighting, co_changingColor;
    //Logica de mostrar.
    private bool IsShowing => CO_Showing != null;
    private bool IsHiding => CO_Hiding != null;
    private bool IsMoving => CO_Moving != null;

    private bool isChangingColor => co_changingColor != null;
    private bool isHighlighting => (highlighted && co_highlighting != null);
    private bool isUnHighlighting => (!highlighted && co_highlighting != null);

    public virtual bool IsVisible => false;

    //Cada personaje tendrá su propio nombre.
    public Character(string name, CharacterConfigData config, GameObject prefab) {
        this.name = name;
        this.config = config;
        displayName = name;

        if(prefab != null)
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab, controller.CharacterPanel);
            obj.SetActive(true);
            root = obj.GetComponent<RectTransform>();
            animator = obj.GetComponentInChildren<Animator>();
        }
        Debug.Log($"Creating character in base: {name}");
    }

    public DialogController DController => DialogController.Instance;

    public void SetDialogColor(Color col) => config.diagCol = col;
    public void SetNameColor(Color col) => config.nameCol = col;

    public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
    public void SetDialogFont(TMP_FontAsset font) => config.diagFont = font;

    public void UpdateConfigOnScreen() => DController.ApplySpeakerDataToBox(config);
    public void ResetCharacterConfig() => config = CharacterController.Instance.GetCharacterConfig(name);

    //Hacer que el personaje hable.
    public Coroutine Say(string dialog) => Say(new List<string> { dialog});
    public Coroutine Say(List <string> dialogLines)
    {
        DController.ShowSpeakerName(displayName);
        UpdateConfigOnScreen();
        return DController.Say(dialogLines);
    }

    public virtual Coroutine Show()
    {
        if (IsShowing)
            return CO_Showing;

        if(IsHiding)
            controller.StopCoroutine(CO_Hiding);

        CO_Showing = controller.StartCoroutine(HandleShowing(true));

        return CO_Showing;
    }

    public virtual Coroutine Hide()
    {
        if (IsHiding)
            return CO_Hiding;

        if (IsShowing)
            controller.StopCoroutine(CO_Showing);

        CO_Hiding = controller.StartCoroutine(HandleShowing(false));

        return CO_Hiding;
    }

    //Estos elementos no corresponden a esta clase.
    public virtual void SetPos(Vector2 pos) {
        if (root == null) return;
        (Vector2 minAnchor, Vector2 maxAnchor) = ConvertPosToRelative(pos);
        root.anchorMin = minAnchor;
        root.anchorMax = maxAnchor;
    }

    public virtual Coroutine MoveToPosition(Vector2 pos, float speed = 2f, bool smooth = false)
    {
        if (root == null) return null;
        if (IsMoving) controller.StopCoroutine(CO_Moving);

        CO_Moving = controller.StartCoroutine(MoveToPositionCoroutine(pos, speed, smooth));

        return CO_Moving;
    }

    private IEnumerator MoveToPositionCoroutine(Vector2 pos, float speed = 2f, bool smooth = false)
    {
        (Vector2 minAnch, Vector2 maxAnch) = ConvertPosToRelative(pos);
        Vector2 padding = root.anchorMax - root.anchorMin;

        while(root.anchorMin != minAnch ||  root.anchorMax != maxAnch)
        {
            root.anchorMin = smooth ? Vector2.Lerp(root.anchorMin, minAnch, speed * Time.deltaTime)
                : Vector2.MoveTowards(root.anchorMin, minAnch, speed * Time.deltaTime * 0.35f);

            root.anchorMax = root.anchorMin + padding;

            if (smooth && Vector2.Distance(root.anchorMin, root.anchorMax) <= 0.001f) {
                root.anchorMin = minAnch;
                root.anchorMax = maxAnch;
                break;
            }

            yield return null;
        }

        Debug.Log($"Finalización de movimiento.");
        CO_Moving = null;
    }

    protected (Vector2, Vector2) ConvertPosToRelative(Vector2 pos)
    {
       //Vector normalizado ya que es una posición relativa como en cualquier juego.
       //Podria dar ejemplos de esto, por ejemplo un cuadrado dentro de un panel.
        Vector2 padding = root.anchorMax - root.anchorMin;
        float maxX = 1 - padding.x;
        float maxY = 1 - padding.y;

        Vector2 minAnchorTgt = new Vector2(maxX * pos.x, maxY * pos.y);
        Vector2 maxAnchorTgt = minAnchorTgt + padding;

        return (minAnchorTgt, maxAnchorTgt);

    }

    public virtual IEnumerator HandleShowing(bool shouldShow) {
        //Debug.LogWarning("Can't be called on abstract character class");
        yield return null;
    }

 
    public virtual void SetColor(Color color)
    {
        this.color = color;
    }
    public Coroutine TransitionColor(Color color, float speed = 1f)
    {
        this.color = color;

        if (isChangingColor)
            controller.StopCoroutine(co_changingColor);

        co_changingColor = controller.StartCoroutine(ChangingColor(displayColor, speed));
        return co_changingColor;
    }
    public virtual IEnumerator ChangingColor(Color color, float speed)
    {
        Debug.LogWarning("Color changing is not applicable on this character type!");
        yield return null;
    }

  
    public Coroutine Highlight(float speed = 1f)
    {
        if (isHighlighting)
            return co_highlighting;

        if (isUnHighlighting)
            controller.StopCoroutine(co_highlighting);

        highlighted = true;
        co_highlighting = controller.StartCoroutine(Highlighting(highlighted, speed));

        return co_highlighting;
    }

    public Coroutine UnHighlight(float speed = 1f)
    {
        if (isUnHighlighting)
            return co_highlighting;

        if (isHighlighting)
            controller.StopCoroutine(co_highlighting);

        highlighted = false;
        co_highlighting = controller.StartCoroutine(Highlighting(highlighted, speed));

        return co_highlighting;
    }

    public virtual IEnumerator Highlighting(bool highlight, float speedMultiplier)
    {
        Debug.Log("Highlighting is not available on this character type!");
        yield return null;
    }

    public enum CharacterType
    {
        //Los mas usados en VN.
        Text,
        Sprite,
        SpriteSheet,
    }
}
