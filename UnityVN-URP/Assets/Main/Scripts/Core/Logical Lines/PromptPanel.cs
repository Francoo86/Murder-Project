using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptPanel : MonoBehaviour
{
    public static PromptPanel Instance { get; private set; }
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController cgController;
    public string LastInput { get; private set; } = string.Empty;
    public bool IsWaitingOnUserInput { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cgController = new CanvasGroupController(this, canvasGroup);

        canvasGroup.alpha = 0;
        SetCanvasState(false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        cgController.Show();
        SetCanvasState(true);
        IsWaitingOnUserInput = true;
    }

    public void Hide() {
        cgController.Hide();
        SetCanvasState(false);
        IsWaitingOnUserInput = false;
    }

    private void SetCanvasState(bool active)
    {
        canvasGroup.blocksRaycasts = active;
        canvasGroup.interactable = active;
    }

    public void OnAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;

        LastInput = inputField.text;
        Hide();
    }

    public void OnInputChanged(string pHolder)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }

    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
}
