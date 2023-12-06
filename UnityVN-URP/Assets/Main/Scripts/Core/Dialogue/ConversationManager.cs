using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager
{
    private Coroutine process = null;
    private TextArchitect arch;
    public bool IsRunning => process != null;

    private DialogController Controller => DialogController.Instance;

    public ConversationManager(TextArchitect arch) { 
        this.arch = arch;
        Controller.onUserPrompt_Next += OnUserPrompt_Next;
    }

    private bool isUserManipulated = false;
    private void OnUserPrompt_Next() {
        isUserManipulated = true;
    }

    public Coroutine StartConversation(List<string> conversation) {
        StopConversation();

        process = Controller.StartCoroutine(RunningConversation(conversation));
        return process;
    }

    public void StopConversation()
    {
        if (!IsRunning) return;

        Controller.StopCoroutine(process);
        process = null;
    }

    IEnumerator RunningConversation(List <string> conversation)
    {
        for(int i = 0; i < conversation.Count; i++)
        {
            string currentConversation = conversation[i];
            if(currentConversation == string.Empty) continue;

            DialogLineModel dialogLine = DialogParser.Parse(currentConversation);

            //Revisamos si tenemos dialogo.
            if (dialogLine.HasDialog) { 
                yield return RunDialogForLine(dialogLine);
            }

            //Revisamos si tenemos comandos a ejecutar.
            if (dialogLine.HasCommands) {
                yield return RunDialogForCommands(dialogLine);
            }

            if (dialogLine.HasDialog)
            {
                yield return WaitForUserInput();

                CommandController.Instance.StopAllProcesses();
            }
            //yield return new WaitForSeconds(1);

        }
    }

    private void HandleSpeaker(SpeakerModel speakerModel)
    {
        bool shouldBeCreated = (speakerModel.MakeCharacterEnter || speakerModel.IsGoingToScreenPos || speakerModel.IsDoingAnyExpression);
        Character character = CharacterController.Instance.GetCharacter(speakerModel.name, shouldBeCreated);

        if (speakerModel.MakeCharacterEnter && (!character.IsVisible))
            character.Show();
            
        //Muestra el nombre del personaje.
        Controller.ShowSpeakerName(speakerModel.DisplayName);

        //Carga los datos creados por la configuración escrita en su clase.
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
            

        //Construir el dialogo.
        //yield return BuildDialogue(dialogLine.dialog);
        yield return BuildLineSegments(dialogLine.dialogData);

        //Esperar al input de usuario, así como tocar la pantalla o cosas así.
        //yield return WaitForUserInput();
    }

    IEnumerator RunDialogForCommands(DialogLineModel dialogLine) {
        List<CommandData.Command> commands = dialogLine.commandData.commands;

        foreach (CommandData.Command command in commands) {
            //Ejecutar esto primero antes de empezar con el otro comando.
            //Añadir check de lowercase commo estos comandos a veces no funcan.
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

    IEnumerator WaitForDialogSegmentSignalToBeTriggered(DialogData.DIALOG_SEGMENT segment)
    {
        switch (segment.startSignal)
        {
            case DialogData.DIALOG_SEGMENT.StartSignal.C:
            case DialogData.DIALOG_SEGMENT.StartSignal.A:
                yield return WaitForUserInput(); 
                break;
            case DialogData.DIALOG_SEGMENT.StartSignal.WC:
            case DialogData.DIALOG_SEGMENT.StartSignal.WA:
                yield return new WaitForSeconds(segment.signalDelay);
                break;
        }
    }

    IEnumerator BuildDialog(string diag, bool append = false) {
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
