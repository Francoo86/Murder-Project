using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class UIConfirmationMenu : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private LayoutGroup choiceLayoutGroup;
    [SerializeField] private GameObject buttonPrefab;

    private GameObject[] activeOptions = new GameObject[0];

    public void Show(string title, params ConfirmationButton[] options)
    {
        if (options.Length == 0)
        {
            UnityEngine.Debug.LogError("Confirmation menu must have at leat 1 option provided for the use to select.");
            return;
        }

        this.title.text = title;

        CreateOptionButtons(options);

        anim.Play("Enter");
    }

    public void Hide()
    {
        anim.Play("Exit");
    }

    private void CreateOptionButtons(ConfirmationButton[] options)
    {
        foreach (GameObject g in activeOptions)
            DestroyImmediate(g);

        activeOptions = new GameObject[options.Length];

        for (int i = 0; i < options.Length; i++)
        {
            ConfirmationButton option =  options[i];
            GameObject ob = Instantiate(buttonPrefab, choiceLayoutGroup.transform);
            ob.SetActive(true);

            Button button = ob.GetComponent<Button>();

            if (option.action != null)
                button.onClick.AddListener(() => option.action.Invoke());

            if (option.autoCloseOnQuit)
                button.onClick.AddListener(() => Hide());

            TextMeshProUGUI txt = ob.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = option.title;

            activeOptions[i] = ob;
        }

    }

    public struct ConfirmationButton
    {
        public string title;
        public System.Action action;
        public bool autoCloseOnQuit;

        public ConfirmationButton(string title, System.Action action, bool autoCloseOnClick = true)
        {
            this.title = title;
            this.action = action;
            this.autoCloseOnQuit = autoCloseOnClick;
        }

    }

}
