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
                yield return RunDialogueForLine(dialogLine);
            }

            //Revisamos si tenemos comandos a ejecutar.
            if (dialogLine.HasCommands) {
                yield return RunDialogueForCommands(dialogLine);
            }

            if(dialogLine.HasDialog) 
                yield return WaitForUserInput();
            //yield return new WaitForSeconds(1);

        }
    }

    IEnumerator RunDialogueForLine(DialogLineModel dialogLine)
    {
        //Muestra o esconde un hablante existente.
        //Ya se configuro el apartado para narrador.
        if (dialogLine.HasSpeaker)
            Controller.ShowSpeakerName(dialogLine.speakerMdl.DisplayName);

        //Construir el dialogo.
        //yield return BuildDialogue(dialogLine.dialog);
        yield return BuildLineSegments(dialogLine.dialogData);

        //Esperar al input de usuario, as� como tocar la pantalla o cosas as�.
        //yield return WaitForUserInput();
    }

    IEnumerator RunDialogueForCommands(DialogLineModel dialogLine) {
        List<CommandData.Command> commands = dialogLine.commandData.commands;

        foreach (CommandData.Command command in commands) {
            //Ejecutar esto primero antes de empezar con el otro comando.
            if (command.waitToFinish)
                yield return CommandController.Instance.Execute(command.name, command.arguments);
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
            yield return BuildDialogue(segment.dialog, segment.ShouldAppend);
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

    IEnumerator BuildDialogue(string diag, bool append = false) {
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

    IEnumerator WaitForUserInput() {
        while (!isUserManipulated) {
            yield return null;
        }

        isUserManipulated = false;
    }
}
