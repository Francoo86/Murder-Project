using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Makes the text run automatically or be skippable in the dialog box.
/// Can be stopped by user pressiing buttons.
/// </summary>
public class AutoReader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    private const int DEFAULT_READ_CHARACTERS_PER_SEC = 18;
    private const float READ_TIME_PADDING = 0.5f;
    private const float MAX_READ_TIME = 99f;
    private const float MIN_READ_TIME = 1f;

    private const string STATUS_TEXT_AUTO = "Auto";
    private const string STATUS_TEXT_SKIP = "Skipping";

    private ConversationManager convManager;
    private TextArchitect Architect => convManager.arch;

    public bool Skip { get; set; } = false;
    public float Speed { get; set; } = 1f;

    private Coroutine co_Running = null;
    public bool IsOn => co_Running != null;

    [HideInInspector] public bool allowToggle = true;

    /// <summary>
    /// Initializes the AutoReader, by passing the Conversation Manager.
    /// </summary>
    /// <param name="convManager">The Conversation manager.</param>
    public void Initialize(ConversationManager convManager)
    {
        this.convManager = convManager;
        statusText.text = string.Empty;
    }

    /// <summary>
    /// Enables the auto status.
    /// </summary>
    public void Enable()
    {
        if (IsOn)
            return;

        co_Running = StartCoroutine(AutoRead());
    }

    /// <summary>
    /// Disables the auto or skip state by resetting it.
    /// </summary>
    public void Disable()
    {
        if(!IsOn) return;

        StopCoroutine(co_Running);
        Skip = false;
        co_Running = null;
        statusText.text = string.Empty;

    }

    /// <summary>
    /// Handles the logic of the AutoRead text also approaching the user input.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoRead()
    {
        if (!convManager.IsRunning)
        {
            Disable();
            yield break;
        }

        //Asegurarse de que se pueda leer bien.
        if(!Architect.isBuilding && Architect.currentText != string.Empty)
        {
            DialogController.Instance.OnSystemPrompt_Next();
        }

        while (convManager.IsRunning)
        {
            if (!Skip)
            {
                while (!Architect.isBuilding && !convManager.IsWaitingOnAutoTimer)
                {
                    yield return null;
                }

                float timeStarted = Time.time;

                while (Architect.isBuilding || convManager.IsWaitingOnAutoTimer)
                {
                    yield return null;
                }

                float characterCount = Architect.tmpro.textInfo.characterCount;
                float timeToRead = Mathf.Clamp(characterCount / DEFAULT_READ_CHARACTERS_PER_SEC, MIN_READ_TIME, MAX_READ_TIME);
                timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), MIN_READ_TIME, MAX_READ_TIME);
                timeToRead = (timeToRead / Speed) + READ_TIME_PADDING;

                yield return new WaitForSeconds(timeToRead);
            }
            else
            {
                Architect.ForceComplete();
                yield return new WaitForSeconds(0.05f);
            }

            DialogController.Instance.OnSystemPrompt_Next();
        }

        Disable();
    }

    public void ToggleAuto()
    {
        if (!allowToggle)
            return;

        if (Skip)
            Enable();
        else
        {
            if (IsOn)
                Disable();
            else
                Enable();
        }

        Skip = false;
        if (IsOn)
            statusText.text = STATUS_TEXT_AUTO;
    }

    public void ToggleSkip()
    {
        if (!allowToggle)
            return;

        if (!Skip)
            Enable();
        else
        {
            if (IsOn)
                Disable();
            else
                Enable();
        }

        Skip = true;
        if (IsOn)
            statusText.text = STATUS_TEXT_SKIP;
    }
}
