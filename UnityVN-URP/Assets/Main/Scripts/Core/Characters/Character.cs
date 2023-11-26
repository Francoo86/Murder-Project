using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Character
{
    public string name;
    public string displayName;
    //Hacer un cuadro para cada imagen de personaje.
    public RectTransform root;
    public CharacterConfigData config;
    public Animator animator;
    protected CharacterController controller => CharacterController.Instance;

    //Corutinas de mostrado.
    protected Coroutine CO_Hiding, CO_Showing;
    //Logica de mostrar.
    private bool IsShowing => CO_Showing != null;
    private bool IsHiding => CO_Hiding != null;
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

    public virtual IEnumerator HandleShowing(bool shouldShow) {
        Debug.LogWarning("Can't be called on abstract character class");
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
