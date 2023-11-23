using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager
{
    private Coroutine process = null;
    private TextArchitect arch;
    public bool isRunning => process != null;

    //TODO: Fix this shitty coupling.
    private DialogController Controller => DialogController.Instance;

    public ConversationManager(TextArchitect arch) { 
        this.arch = arch;
        Controller.onUserPrompt_Next += OnUserPrompt_Next;
    }

    private bool isUserManipulated = false;
    private void OnUserPrompt_Next() {
        isUserManipulated = true;
    }

    public void StartConversation(List<string> conversation) {
        Debug.Log($"Object check {conversation == null}");
        StopConversation();

        process = Controller.StartCoroutine(RunningConversation(conversation));
    }

    public void StopConversation()
    {
        if (!isRunning) return;

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

            //yield return new WaitForSeconds(1);

        }
    }

    IEnumerator RunDialogueForLine(DialogLineModel dialogLine)
    {
        //Muestra o esconde un hablante existente.
        //Ya se configuro el apartado para narrador.
        if (dialogLine.HasSpeaker)
            Controller.ShowSpeakerName(dialogLine.speaker);

        //Construir el dialogo.
        //yield return BuildDialogue(dialogLine.dialog);
        yield return BuildLineSegments(dialogLine.dialog);

        //Esperar al input de usuario, así como tocar la pantalla o cosas así.
        yield return WaitForUserInput();
    }

    IEnumerator RunDialogueForCommands(DialogLineModel dialogLine) {
        Debug.Log(dialogLine.commands);
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
