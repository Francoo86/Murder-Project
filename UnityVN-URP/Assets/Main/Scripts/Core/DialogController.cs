using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using CHARACTERS;

/// <summary>
/// Controls the dialog stuff appearing the conversation, like clicking on screen and everything related to the dialog box.
/// </summary>
public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogConfig _config;
    [SerializeField] private CanvasGroup canvas;
    private const string NARRATOR_CHARACTER = "narrator";

    public DialogConfig Config => _config;

    public DialogContainer dialogContainer = new DialogContainer();
    public ConversationManager convManager;
    public static DialogController Instance { get; private set; }
    private TextArchitect architect;
    private CanvasGroupController CGController;
    public AutoReader autoReader;

    //Definimos una "funcion" que es m�s o menos personalizable.
    public delegate void DialogSystemEvent();
    //Esta definicion de evento se comporta como el patr�n Observador.
    //Lo que haremos con el es llamarlo cada vez que hagamos click en algo.
    public event DialogSystemEvent onUserPrompt_Next;
    public event DialogSystemEvent onClear;

    //El 'prompt' que aparece aqui, fue removido debido a limitaciones de tiempo.

    public bool IsRunning => convManager.IsRunning;

    //Inicializa el objeto en el script.

    /// <summary>
    /// Initializes a singleton instance.
    /// </summary>
    void Awake()
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

    /// <summary>
    /// Initializes the controller by creating the controllers/managers, like the conversation and the CanvasGroup controller.
    /// </summary>
    public void Initialize() { 
        if(_hasInitialized) return;

        architect = new TextArchitect(dialogContainer.dialogText);
        convManager = new ConversationManager(architect);

        CGController = new CanvasGroupController(this, canvas);
        dialogContainer.Initialize();

        _hasInitialized = true;
        autoReader = GetComponent<AutoReader>();

        if (autoReader != null)
            autoReader.Initialize(convManager);
    }

    /// <summary>
    /// This is called when user clicks the screen while the dialog is displaying.
    /// </summary>
    public void OnUserPrompt_Next() {
        //Debug.Log("Invoking...");
        if (InworldWrapper.Instance.IsFetching)
            return;

        //Si no es nulo lo invoca.
        onUserPrompt_Next?.Invoke();
        
        if(autoReader != null && autoReader.IsOn)
        {
            autoReader.Disable();
        }
    }

    /// <summary>
    /// Called when the text is cleared automatically by the game.
    /// </summary>
    public void OnSystemPrompt_Clear()
    {
        onClear?.Invoke();
    }

    /// <summary>
    /// This function is called when the player looks throughout the log history.
    /// </summary>
    public void OnStartViewingHistory()
    {
        //prompt.Hide();
        autoReader.allowToggle = false;
        convManager.allowUserPrompts = false;

        if (autoReader.IsOn) 
        {
            autoReader.Disable();
        }
    }

    /// <summary>
    /// This function is called when the player closes the history log.
    /// </summary>
    public void OnStopViewingHistory()
    {
        //prompt.Show();
        autoReader.allowToggle = true;
        convManager.allowUserPrompts = true;
    }

    public void OnSystemPrompt_Next()
    {
        //Debug.Log("Invoking...");
        //Si no es nulo lo invoca.
        onUserPrompt_Next?.Invoke();
    }


    /// <summary>
    /// Applies the character configuration data to be displayed on the screen. Like the text color or dialog color associated to it.
    /// </summary>
    /// <param name="config">The character configuration data in the file.</param>
    public void ApplySpeakerDataToBox(CharacterConfigData config)
    {
        dialogContainer.SetDialogColor(config.diagCol);
        dialogContainer.SetDialogFont(config.diagFont);

        dialogContainer.nameContainer.SetNameColor(config.nameCol);
        dialogContainer.nameContainer.SetNameFont(config.nameFont);
    }

    //Le hace un lookup a la configuraci�n de los personajes solamente pasandole el nombre como parametro.
    /// <summary>
    /// Overload of the method but instead passing the character name.
    /// </summary>
    /// <param name="speakerName">The character name.</param>
    public void ApplySpeakerDataToBox(string speakerName) {
        Character character = CharacterController.Instance.GetCharacter(speakerName);
        CharacterConfigData config = character != null ? character.config : CharacterController.Instance.GetCharacterConfig(speakerName);
        ApplySpeakerDataToBox(config);
    }

    /// <summary>
    /// Shows the character name on the screen, if the character is the narrator it wil not show it.
    /// </summary>
    /// <param name="speakerName">The character name.</param>
    public void ShowSpeakerName(string speakerName = "")
    {
        //No hay raz�n para mostrar al narrador, similar a RenPy.
        if (speakerName.ToLower() != NARRATOR_CHARACTER)
            dialogContainer.nameContainer.Show(speakerName);
        else { 
            HideSpeakerName();
            dialogContainer.nameContainer.nameText.text = "";
        }
    }

    /// <summary>
    /// Hides the name label of the character.
    /// </summary>
    public void HideSpeakerName() => dialogContainer.nameContainer.Hide();
    
    //Metodos sobrecargados para decir algo.
    //Este es más que nada si no contamos con hablante.

    /// <summary>
    /// Overloaded method that needs only the speaker and dialog to convert it into a list.
    /// </summary>
    /// <param name="speaker">The character name.</param>
    /// <param name="dialogue">The dialogue associated to the character.</param>
    /// <returns>The coroutine process asociated to the character.</returns>
    public Coroutine Say(string speaker, string dialogue) {
        List<string> conversation = new List<string>() {$"{speaker} \"{dialogue}\""};
        return Say(conversation);
    }

    /// <summary>
    /// Starts the conversation by passing the file lines, or a list of quotes that have valid syntax for elements.
    /// And creates a conversation object.
    /// </summary>
    /// <param name="lines">The dialog lines.</param>
    /// <param name="filePath">The path of the read file.</param>
    /// <returns>The Coroutine associated to the conversation process.</returns>
    public Coroutine Say(List<string> lines, string filePath="") {
        Conversation conversation = new Conversation(lines, file: filePath);
        return convManager.StartConversation(conversation);
    }

    /// <summary>
    /// Runs the conversation process by passing a conversation object instead.
    /// </summary>
    /// <param name="conversation">The conversation object.</param>
    /// <returns>The coroutine associated  to the conversation process.</returns>
    public Coroutine Say(Conversation conversation)
    {
        return convManager.StartConversation(conversation);
    }

    //THIS SHOULD BE IMPLEMENT ON A GENERIC INTERFACE.
    /// <summary>
    /// Shows the screen UI.
    /// </summary>
    /// <param name="speed">How fast the transition should be.</param>
    /// <param name="inmediate">Skip the speed and do it instantly.</param>
    /// <returns>The Coroutine process associated to Show inside the CGController.</returns>
    public Coroutine Show(float speed, bool inmediate) => CGController.Show(speed, inmediate);
    /// <summary>
    /// Hide the screen UI.
    /// </summary>
    /// <param name="speed">How fast the transition should be.</param>
    /// <param name="inmediate">Skip the speed and do it instantly.</param>
    /// <returns>The Coroutine process associated to Hide inside the CGController.</returns>
    public Coroutine Hide(float speed, bool inmediate) => CGController.Hide(speed, inmediate);
    /// <summary>
    /// Checks if the screen is active.
    /// </summary>
    public bool IsVisible => CGController.IsVisible;
}