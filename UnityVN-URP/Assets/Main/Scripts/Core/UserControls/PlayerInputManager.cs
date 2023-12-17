using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using History;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput input;
    //Estas acciones son asignadas en un archivo y en un cambio de escena PODRIAN apuntar a referencias nulas.
    //OJO.
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext>)> ();
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        InitializeActions();
    }

    private void InitializeActions()
    {
        actions.Add((input.actions["Next"], PromptAdvance));
        actions.Add((input.actions["HistoryBack"], OnHistoryBack));
        actions.Add((input.actions["historyFoward"], OnHistoryFoward));
    }

    /// <summary>
    /// Funcion que se ejecuta al momento de cambiar escena.
    /// </summary>
    private void OnEnable()
    {
        foreach(var inputAct in actions)
            inputAct.action.performed += inputAct.command;
    }

    private void OnDisable()
    {
        foreach (var inputAct in actions)
            inputAct.action.performed -= inputAct.command;
    }
    public void PromptAdvance(InputAction.CallbackContext c) {
        DialogController.Instance.OnUserPrompt_Next();
    }

    public void OnHistoryBack(InputAction.CallbackContext c)
    {
        HistoryManager.Instance.GoBack();
    }

    public void OnHistoryFoward(InputAction.CallbackContext c)
    {
        HistoryManager.Instance.GoFoward();
    }
}
