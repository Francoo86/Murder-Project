using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CHARACTERS;

public class ConversationManager
{
    private Coroutine process = null;
    public TextArchitect arch;
    public bool IsRunning => process != null;

    private ConversationQueue convQueue;
    private DialogController Controller => DialogController.Instance;
    //Para inyectar variables en su caso de tipo XML.
    //private TagController tagController;
    private LogicalLineManager logicalLineManager;
    public ConversationManager(TextArchitect arch) { 
        this.arch = arch;
        Controller.onUserPrompt_Next += OnUserPrompt_Next;

        //tagController = new TagController();
        logicalLineManager = new LogicalLineManager();
        convQueue = new ConversationQueue();
    }

    public bool allowUserPrompts = true;

    public void Enqueue(Conversation conversation) => convQueue.Enqueue(conversation);
    public void EnqueuePriority(Conversation conversation) => convQueue.EnqueuePriority(conversation);

    private bool isUserManipulated = false;

    private void OnUserPrompt_Next() {
        if(allowUserPrompts)
            isUserManipulated = true;
    }

    public Coroutine StartConversation(Conversation conversation) {
        StopConversation();

        convQueue.Clear();

        Enqueue(conversation);

        process = Controller.StartCoroutine(RunningConversation());
        return process;
    }

    public void StopConversation()
    {
        if (!IsRunning) return;

        Controller.StopCoroutine(process);
        process = null;
    }

    public Conversation conversation => (convQueue.IsEmpty() ? null : convQueue.top);
    //TODO: Refactorizar un poco si es posible.
    public int conversationProgress => (convQueue.IsEmpty() ? -1 : convQueue.top.GetProgress());

    IEnumerator RunningConversation()
    {
        //for(int i = 0; i < conversation.Count; i++)
        while(!convQueue.IsEmpty())
        {
            Conversation currentConversation = conversation;

            if (currentConversation.HasReachedEnd())
            {
                convQueue.Dequeue();
                continue;
            }

            string rawLine = currentConversation.GetCurrentLine();
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                TryAdvanceConversation(currentConversation);
                continue;
            }

            DialogLineModel line = DialogParser.Parse(rawLine);

            if (logicalLineManager.TryGetLogic(line, out Coroutine logic)) {
                yield return logic;
            }
            else
            {
                //Revisamos si tenemos dialogo.
                if (line.HasDialog)
                {
                    yield return RunDialogForLine(line);
                }

                //Revisamos si tenemos comandos a ejecutar.
                if (line.HasCommands)
                {
                    yield return RunDialogForCommands(line);
                }

                if (line.HasDialog)
                {
                    yield return WaitForUserInput();

                    CommandController.Instance.StopAllProcesses();

                    DialogController.Instance.OnSystemPrompt_Clear();
                }
            }

            TryAdvanceConversation(currentConversation);

            //yield return new WaitForSeconds(1);

        }

        process = null;
    }

    private void TryAdvanceConversation(Conversation conversation)
    {
        conversation.IncrementProgress();

        if (conversation != convQueue.top)
            return;

        if (conversation.HasReachedEnd())
            convQueue.Dequeue();
    }

    private void HandleSpeaker(SpeakerModel speakerModel)
    {
        bool shouldBeCreated = (speakerModel.MakeCharacterEnter || speakerModel.IsGoingToScreenPos || speakerModel.IsDoingAnyExpression);
        Character character = CharacterController.Instance.GetCharacter(speakerModel.name, shouldBeCreated);

        if (speakerModel.MakeCharacterEnter && (!character.IsVisible))
            character.Show();
            
        //Muestra el nombre del personaje.
        Controller.ShowSpeakerName(TagController.Inject(speakerModel.DisplayName));

        //Carga los datos creados por la configuraci�n escrita en su clase.
        Controller.ApplySpeakerDataToBox(speakerModel.name);

        if (speakerModel.IsGoingToScreenPos)
            character.MoveToPosition(speakerModel.speakerScrPos);

        if(speakerModel.IsDoingAnyExpression)
            foreach(var exp in speakerModel.ScreenExpressions)
            {
                //TODO: Remove the layer thing as we are not working with that.
                character.OnExpressionReceive(exp.layer, exp.expression);
            }
    }
    IEnumerator RunDialogForLine(DialogLineModel dialogLine)
    {
        //Muestra o esconde un hablante existente.
        //Ya se configuro el apartado para narrador.
        if (dialogLine.HasSpeaker)
        {
            HandleSpeaker(dialogLine.speakerMdl);
            //Mostrar al personaje en la interfaz.
        }

        if (!Controller.dialogContainer.IsVisible)
            Controller.dialogContainer.Show();

        //Construir el dialogo.
        //yield return BuildDialogue(dialogLine.dialog);
        yield return BuildLineSegments(dialogLine.dialogData);

        //Esperar al input de usuario, as� como tocar la pantalla o cosas as�.
        //yield return WaitForUserInput();
    }

    IEnumerator RunDialogForCommands(DialogLineModel dialogLine) {
        List<CommandData.Command> commands = dialogLine.commandData.commands;

        foreach (CommandData.Command command in commands) {
            //Ejecutar esto primero antes de empezar con el otro comando.
            //A�adir check de lowercase commo estos comandos a veces no funcan.
            //TODO: Re-considerar si esta medida es necesaria.
            if (command.waitToFinish || command.name.ToLower() == "wait")
            {
                //Is this really a RenPy 2 UNAP edition?
                CoroutineWrapper wrap = CommandController.Instance.Execute(command.name, command.arguments);

                while (!wrap.IsDone)
                {
                    if (isUserManipulated)
                    {
                        CommandController.Instance.StopCurrentProcess();
                        isUserManipulated = false;
                    }

                    //If this is not on the loop this causes stack overflow.
                    yield return null;
                }
                //yield return CommandController.Instance.Execute(command.name, command.arguments);
            }
            else
                CommandController.Instance.Execute(command.name, command.arguments);
        } 
        //Debug.Log(dialogLine.commandData);
        yield return null;
    }

    IEnumerator BuildLineSegments(DialogData dialogLine) {
        for(int i = 0; i < dialogLine.segments.Count; i++)
        {
            DialogData.DIALOG_SEGMENT segment = dialogLine.segments[i];

            yield return WaitForDialogSegmentSignalToBeTriggered(segment);
            yield return BuildDialog(segment.dialog, segment.ShouldAppend);
        }
    }

    public bool IsWaitingOnAutoTimer { get; private set; }
    IEnumerator WaitForDialogSegmentSignalToBeTriggered(DialogData.DIALOG_SEGMENT segment)
    {
        switch (segment.startSignal)
        {
            case DialogData.DIALOG_SEGMENT.StartSignal.C:
                yield return WaitForUserInput();
                Controller.OnSystemPrompt_Clear();
                break;
            case DialogData.DIALOG_SEGMENT.StartSignal.A:
                yield return WaitForUserInput(); 
                break;
            case DialogData.DIALOG_SEGMENT.StartSignal.WC:
                IsWaitingOnAutoTimer = true;
                yield return new WaitForSeconds(segment.signalDelay);
                IsWaitingOnAutoTimer = false;
                Controller.OnSystemPrompt_Clear();
                break;
            case DialogData.DIALOG_SEGMENT.StartSignal.WA:
                IsWaitingOnAutoTimer = true;
                yield return new WaitForSeconds(segment.signalDelay);
                IsWaitingOnAutoTimer = false;
                break;
        }
    }

    IEnumerator BuildDialog(string diag, bool append = false) {
        diag = TagController.Inject(diag);

        if (!append)
            arch.Build(diag);
        else
            arch.Append(diag);

        //Esperar a que el dialogo se termine de construir.
        while (arch.isBuilding)
        {
            if (isUserManipulated)
            {
                if (!arch.shouldSpeedUp) arch.shouldSpeedUp = true;
                else arch.ForceComplete();

                isUserManipulated = false;
            }

            yield return null;
        }
    }

    //TODO: Redux this thing.
    IEnumerator WaitForUserInput() {
        while (!isUserManipulated) {
            yield return null;
        }

        isUserManipulated = false;
    }
}
