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
        if (dialogLine.HasSpeaker)
            Controller.ShowSpeakerName(dialogLine.speaker);
        else
            Controller.HideSpeakerName();

        //Construir el dialogo.
        yield return BuildDialogue(dialogLine.dialog);

        //Esperar al input de usuario, así como tocar la pantalla o cosas así.
        yield return WaitForUserInput();
    }

    IEnumerator RunDialogueForCommands(DialogLineModel dialogLine) {
        Debug.Log(dialogLine.commands);
        yield return null;
    }

    IEnumerator BuildDialogue(string diag) {
        arch.Build(diag);

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
