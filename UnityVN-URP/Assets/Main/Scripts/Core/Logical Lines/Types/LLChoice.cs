using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LogicalLineUtils.Encapsulation;

public class LLChoice : ILogicalLine
{
    public string Keyword => "choice";
    private const char CHOICE_IDENTIFIER = '-';

    public IEnumerator Execute(DialogLineModel line)
    {
        ConversationManager ConvManager = DialogController.Instance.convManager;
        Conversation currentConversation = ConvManager.conversation;
        int currentProgress = ConvManager.conversationProgress;
        //Necesitamos el titulo.
        EncapsulationData data = RipEncapsulationData(currentConversation, currentProgress, true);
        List<Choice> choices = GetChoicesFromData(data);

        string title = line.dialogData.RawData;
        ChoicePanel panel = ChoicePanel.Instance;
        string[] choiceTitles = choices.Select(x => x.title).ToArray();

        panel.Show(title, choiceTitles);

        while(panel.IsWaitingOnUserChoice)
            yield return null;

        Choice selectedChoice = choices[panel.LastDecision.answerIndex];

        //Evitar que las conversaciones se solapen dentro de la cola.
        Conversation newConversation = new Conversation(selectedChoice.resultLines);
        ConvManager.conversation.SetProgress(data.endingIndex);
        ConvManager.EnqueuePriority(newConversation);
    }

    private List<Choice> GetChoicesFromData(EncapsulationData data)
    {
        List<Choice> choices = new List<Choice>();
        int encapsulationDepth = 0;
        bool IsFirstChoice = true;

        Choice choice = new Choice
        {
            title = string.Empty,
            resultLines = new List<string>(),
        };

        foreach(var line in data.lines.Skip(1))
        {
            if(IsChoiceStart(line) && encapsulationDepth == 1)
            {
                if(!IsFirstChoice)
                {
                    choices.Add(choice);

                    choice = new Choice
                    {
                        title = string.Empty,
                        resultLines = new List<string>(),
                    };
                }

                choice.title = line.Trim().Substring(1);
                IsFirstChoice = false;
                continue;
            }

            //Esto en C es como utilizar el operador &.
            AddLineToResults(line, ref choice, ref encapsulationDepth);
        }

        if(!choices.Contains(choice))
            choices.Add(choice);

        return choices;
    }

    private void AddLineToResults(string line, ref Choice choice, ref int encapsulationDepth)
    {
        line.Trim();

        if(IsEncapsulationStart(line)) {
            if(encapsulationDepth > 0)
                choice.resultLines.Add(line);
            encapsulationDepth++;
            return;
        }

        if (IsEncapsulationEnd(line))
        {
            encapsulationDepth--;

            if(encapsulationDepth > 0)
            {
                choice.resultLines.Add(line);
            }

            return;
        }

        choice.resultLines.Add(line);
    }

    public bool Matches(DialogLineModel line)
    {
        //throw new System.NotImplementedException();
        return (line.HasSpeaker && line.speakerMdl.name.ToLower() == Keyword);
    }

    private bool IsChoiceStart(string line) => line.Trim().StartsWith(CHOICE_IDENTIFIER);

    private struct Choice
    {
        public string title;
        public List<string> resultLines;
    }
}
