using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogConfig _config;

    public DialogConfig Config => _config;

    public DialogContainer dialogContainer = new DialogContainer();
    private ConversationManager convManager;
    public static DialogController Instance { get; private set; }
    private TextArchitect architect;

    public delegate void DialogSystemEvent();
    public event DialogSystemEvent onUserPrompt_Next;

    public bool IsRunning => convManager.IsRunning;

    //Inicializa el objeto en el script.
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else {
            DestroyImmediate(gameObject);
        }
    }

    bool _hasInitialized = false;

    public void Initialize() { 
        if(_hasInitialized) return;

        architect = new TextArchitect(dialogContainer.dialogText);
        convManager = new ConversationManager(architect);
        _hasInitialized = true;
    }

    public void OnUserPrompt_Next() {
        onUserPrompt_Next?.Invoke();
    }

    public void ShowSpeakerName(string speakerName = "")
    {
        //No hay razón para mostrar al narrador, similar a RenPy.
        if (speakerName.ToLower() != "narrator")
            dialogContainer.nameContainer.Show(speakerName);
        else
            HideSpeakerName();
    }
    public void HideSpeakerName() => dialogContainer.nameContainer.Hide();
    //TODO: Implement Strategy.
    public Coroutine Say(string speaker, string dialogue) {
        List<string> conversation = new List<string>() {$"{speaker} \"{dialogue}\""};
        return Say(conversation);
    }
    public Coroutine Say(List<string> conversation) {
        return convManager.StartConversation(conversation);
    }
}
