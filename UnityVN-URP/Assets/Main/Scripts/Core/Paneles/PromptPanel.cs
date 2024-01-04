using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles the player input panel stuff for the game.
/// </summary>
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
    
    /// <summary>
    /// Setups the instance.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Hides the panel and disables interaction, and adds listeners to the button and the input field.
    /// </summary>
    void Start()
    {
        cgController = new CanvasGroupController(this, canvasGroup);

        canvasGroup.alpha = 0;
        cgController.SetInteractableState(false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    /// <summary>
    /// Shows the panel with title and makes it interactable.
    /// </summary>
    /// <param name="title">The title of the panel.</param>
    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        cgController.Show();
        cgController.SetInteractableState(true);
        IsWaitingOnUserInput = true;
    }

    public void Hide() {
        cgController.Hide();
        cgController.SetInteractableState(false);
        IsWaitingOnUserInput = false;
    }


    /// <summary>
    /// When input is accepted the text will be saved as LastInput and the panel will be hidden.
    /// </summary>
    public void OnAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;

        LastInput = inputField.text;
        Hide();
    }

    /// <summary>
    /// Sets the button active based on the valid text checking.
    /// </summary>
    /// <param name="pHolder"></param>
    public void OnInputChanged(string pHolder)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }

    /// <summary>
    /// Checks if the input field has at least 1 character.
    /// </summary>
    /// <returns>The check result.</returns>
    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
}
