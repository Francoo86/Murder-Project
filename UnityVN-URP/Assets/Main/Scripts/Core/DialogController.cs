using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogConfig _config;
    [SerializeField] private CanvasGroup canvas;
    private const string NARRATOR_CHARACTER = "narrator";

    public DialogConfig Config => _config;

    public DialogContainer dialogContainer = new DialogContainer();
    private ConversationManager convManager;
    public static DialogController Instance { get; private set; }
    private TextArchitect architect;
    private CanvasGroupController CGController;
    private AutoReader autoReader;

    //Definimos una "funcion" que es m�s o menos personalizable.
    public delegate void DialogSystemEvent();
    //Esta definicion de evento se comporta como el patr�n Observador.
    //Lo que haremos con el es llamarlo cada vez que hagamos click en algo.
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

        CGController = new CanvasGroupController(this, canvas);
        dialogContainer.Initialize();

        _hasInitialized = true;

        if (TryGetComponent(out autoReader))
            autoReader.Initialize(convManager);
    }

    public void OnUserPrompt_Next() {
        //Debug.Log("Invoking...");
        //Si no es nulo lo invoca.
        onUserPrompt_Next?.Invoke();
        
        if(autoReader != null && autoReader.IsOn)
        {
            autoReader.Disable();
        }
    }

    public void OnSystemPrompt_Next()
    {
        //Debug.Log("Invoking...");
        //Si no es nulo lo invoca.
        onUserPrompt_Next?.Invoke();
    }


    public void ApplySpeakerDataToBox(CharacterConfigData config)
    {
        dialogContainer.SetDialogColor(config.diagCol);
        dialogContainer.SetDialogFont(config.diagFont);

        dialogContainer.nameContainer.SetNameColor(config.nameCol);
        dialogContainer.nameContainer.SetNameFont(config.nameFont);
    }

    //Le hace un lookup a la configuraci�n de los personajes solamente pasandole el nombre como parametro.
    public void ApplySpeakerDataToBox(string speakerName) {
        Character character = CharacterController.Instance.GetCharacter(speakerName);
        CharacterConfigData config = character != null ? character.config : CharacterController.Instance.GetCharacterConfig(speakerName);
        ApplySpeakerDataToBox(config);
    }
    public void ShowSpeakerName(string speakerName = "")
    {
        //No hay raz�n para mostrar al narrador, similar a RenPy.
        if (speakerName.ToLower() != NARRATOR_CHARACTER)
            dialogContainer.nameContainer.Show(speakerName);
        else
            HideSpeakerName();
    }

    public void HideSpeakerName() => dialogContainer.nameContainer.Hide();
    
    //Metodos sobrecargados para decir algo.
    //Este es más que nada si no contamos con hablante.
    public Coroutine Say(string speaker, string dialogue) {
        List<string> conversation = new List<string>() {$"{speaker} \"{dialogue}\""};
        return Say(conversation);
    }
    public Coroutine Say(List<string> conversation) {
        return convManager.StartConversation(conversation);
    }

    //THIS SHOULD BE IMPLEMENT ON A GENERIC INTERFACE.
    public Coroutine Show(float speed, bool inmediate) => CGController.Show(speed, inmediate);
    public Coroutine Hide(float speed, bool inmediate) => CGController.Hide(speed, inmediate);
    public bool IsVisible => CGController.IsVisible;
}