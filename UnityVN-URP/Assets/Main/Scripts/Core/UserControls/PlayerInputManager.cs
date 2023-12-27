using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using History;

/// <summary>
/// Handles the player input like keyboard, or touching the screen.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput input;
    //Estas acciones son asignadas en un archivo y en un cambio de escena PODRIAN apuntar a referencias nulas.
    //OJO.
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext>)> ();
    /// <summary>
    /// Loads the PlayerInput component and stores the actions defined in the file, and associates each one of them with a method.
    /// </summary>
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        InitializeActions();
    }

    /// <summary>
    /// Stores the inputs with a method defined on them.
    /// </summary>
    private void InitializeActions()
    {
        actions.Add((input.actions["Next"], PromptAdvance));
        actions.Add((input.actions["HistoryBack"], OnHistoryBack));
        actions.Add((input.actions["HistoryFoward"], OnHistoryFoward));
        actions.Add((input.actions["HistoryLogs"], OnHistoryToggleLog));
    }

    /// <summary>
    /// Function that runs when changing scene, it reloads the commands.
    /// </summary>
    private void OnEnable()
    {
        foreach(var inputAct in actions)
            inputAct.action.performed += inputAct.command;
    }

    /// <summary>
    /// Removes the actions when changing the scene.
    /// </summary>

    private void OnDisable()
    {
        foreach (var inputAct in actions)
            inputAct.action.performed -= inputAct.command;
    }

    /// <summary>
    /// This is the method associated to make the dialog advance normally.
    /// </summary>
    /// <param name="c"></param>
    public void PromptAdvance(InputAction.CallbackContext c) {
        if (CoroutinePrompt.GetInstance().IsTalkingWithCharacter)
            return;

        DialogController.Instance.OnUserPrompt_Next();
    }

    /// <summary>
    /// Go back into the history to a few points ago.
    /// </summary>
    /// <param name="c"></param>
    public void OnHistoryBack(InputAction.CallbackContext c)
    {
        HistoryManager.Instance.GoBack();
    }

    /// <summary>
    /// Go forward to a new points.
    /// </summary>
    /// <param name="c"></param>
    public void OnHistoryFoward(InputAction.CallbackContext c)
    {
        HistoryManager.Instance.GoFoward();
    }

    /// <summary>
    /// Opens the history log.
    /// </summary>
    /// <param name="c"></param>
    public void OnHistoryToggleLog(InputAction.CallbackContext c)
    {
        var logs = HistoryManager.Instance.logManager;

        if (!logs.isOpen)
        {
            logs.Open();
        }
        else 
        {
            logs.Close();
        }
    }
}
