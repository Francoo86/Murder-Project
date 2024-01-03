using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The choice panel logic handler.
/// </summary>
public class ChoicePanel : MonoBehaviour
{
    // Start is called before the first frame update
    public static ChoicePanel Instance { get; private set; }
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;

    private const float BUTTON_MIN_WIDTH = 50;
    private const float BUTTON_MAX_WIDTH = 1000;
    private const float BUTTON_WIDTH_PADDING = 25;
    private const float BUTTON_HEIGHT_PER_LINE = 50f;
    private const float BUTTON_HEIGHT_PADDING = 20f;

    private CanvasGroupController cgController = null;
    public bool IsWaitingOnUserChoice { get; private set; } = false;
    public ChoicePanelDecision LastDecision { get; private set; } = null;
    private List<ChoiceButton> buttons = new List<ChoiceButton>();

    /// <summary>
    /// Initializes the instance of this class, this class must be unique as we don't need more logic about choices.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Starts in the game with the panel hidden.
    /// </summary>
    void Start()
    {
        cgController = new CanvasGroupController(this, canvasGroup);
        cgController.Alpha = 0;
        cgController.SetInteractableState(false);
    }

    /// <summary>
    /// Shows the choice panel with the title and the possible choices on the screen.
    /// </summary>
    /// <param name="question">The title of the choice panel, usually a question.</param>
    /// <param name="choices">The array of possible choices to be displayed on the buttons.</param>
    public void Show(string question, string[] choices)
    {
        LastDecision = new ChoicePanelDecision(question, choices);
        IsWaitingOnUserChoice = true;

        cgController.Show();
        cgController.SetInteractableState(true);

        titleText.text = question;
        StartCoroutine(GenerateChoices(choices));
    }

    /// <summary>
    /// Hides the panel and disables the interaction with it.
    /// </summary>
    public void Hide()
    {
        cgController.Hide();
        cgController.SetInteractableState(false);
    }

    private IEnumerator GenerateChoices(string[] choices)
    {
        float maxWidth = 0;

        for(int i = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if(i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObj = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObj.SetActive(true);

                Button newButton = newButtonObj.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, title = newTitle, layout = newLayout };

                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choiceButton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choiceButton.title.preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_WIDTH);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }

        foreach(var button in buttons)
        {
            button.layout.preferredWidth = maxWidth;
        }

        for(int i = 0; i < buttons.Count; i++)
        {
            bool show = i < choices.Length;
            buttons[i].button.gameObject.SetActive(show);
        }

        yield return new WaitForEndOfFrame();

        foreach(var button in buttons)
        {
            button.title.ForceMeshUpdate();
            int lines = button.title.textInfo.lineCount;
            button.layout.preferredHeight = BUTTON_HEIGHT_PADDING + (BUTTON_HEIGHT_PER_LINE * lines);
        }
    }

    public void AcceptAnswer(int index)
    {
        if (index < 0 || index > LastDecision.choices.Length - 1)
            return;

        LastDecision.answerIndex = index;
        IsWaitingOnUserChoice = false;
        Hide();
    }

    private struct ChoiceButton
    {
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }
}
