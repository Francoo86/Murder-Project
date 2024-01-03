using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LogicalLineUtils.Encapsulation;

/// <summary>
/// Logical line of choice, called with 'choice' calls choices in-game by parsing the dialog file.
/// </summary>
public class LLChoice : ILogicalLine
{
    public string Keyword => "choice";
    private const char CHOICE_IDENTIFIER = '-';

    /// <summary>
    /// Executes the choice code, by parsing the decisions and what it does.
    /// </summary>
    /// <param name="line">The line model.</param>
    /// <returns>The IEnumerator to be yielded.</returns>
    public IEnumerator Execute(DialogLineModel line)
    {
        ConversationManager ConvManager = DialogController.Instance.convManager;
        Conversation currentConversation = ConvManager.conversation;
        int currentProgress = ConvManager.conversationProgress;
        //Necesitamos el titulo.
        EncapsulationData data = RipEncapsulationData(currentConversation, currentProgress, true, parentStartingIndex: currentConversation.fileStartIndex);
        List<Choice> choices = GetChoicesFromData(data);

        string title = line.dialogData.RawData;
        ChoicePanel panel = ChoicePanel.Instance;
        string[] choiceTitles = choices.Select(x => x.title).ToArray();

        panel.Show(title, choiceTitles);

        while(panel.IsWaitingOnUserChoice)
            yield return null;

        Choice selectedChoice = choices[panel.LastDecision.answerIndex];

        //Evitar que las conversaciones se solapen dentro de la cola.
        Conversation newConversation = new Conversation(selectedChoice.resultLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);
        int fixedIndex = data.endingIndex - currentConversation.fileStartIndex;

        //Fix for choices not going properly.
        if ((data.endingIndex < currentConversation.fileStartIndex) && fixedIndex < 0)
            fixedIndex = currentConversation.fileStartIndex;

        ConvManager.conversation.SetProgress(fixedIndex);
        ConvManager.EnqueuePriority(newConversation);

        //Esto siempre va a ser falso xd
        DialogController.Instance.TryToStopAutoReader();
    }

    /// <summary>
    /// Gets the choice data or the decision elements encapsulated inside of the brackets.
    /// </summary>
    /// <param name="data">The encapsulated data</param>
    /// <returns>List of choices and their methods.</returns>
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

        int choiceIndex = 0;
        int i = 0;

        for(i = 1; i < data.lines.Count; i++)
        //foreach(var line in data.lines.Skip(1))
        {
            var line = data.lines[i];
            if(IsChoiceStart(line) && encapsulationDepth == 1)
            {
                if(!IsFirstChoice)
                {
                    choice.startIndex = data.startingIndex + (choiceIndex+1);
                    choice.endIndex = data.startingIndex + (i-1);
                    choices.Add(choice);
                    choice = new Choice
                    {
                        title = string.Empty,
                        resultLines = new List<string>(),
                    };
                }

                choiceIndex = i;
                choice.title = line.Trim().Substring(1);
                IsFirstChoice = false;
                continue;
            }

            //Esto en C es como utilizar el operador &.
            AddLineToResults(line, ref choice, ref encapsulationDepth);
        }

        if (!choices.Contains(choice))
        {
            choice.startIndex = data.startingIndex + (choiceIndex + 1);
            choice.endIndex = data.startingIndex + (i - 2);
            choices.Add(choice);
        }

        return choices;
    }

    /// <summary>
    /// Adds every choice and its results inside the choice list.
    /// </summary>
    /// <param name="line">Lines that contains the choices.</param>
    /// <param name="choice">The choice reference to store the results and the data.</param>
    /// <param name="encapsulationDepth">The reference of how nested are the choices.</param>
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

    /// <summary>
    /// Checks if matches the choice keyword.
    /// </summary>
    /// <param name="line">The dialog line model.</param>
    /// <returns>Wether the line matches with the keyword or not.</returns>
    public bool Matches(DialogLineModel line)
    {
        //throw new System.NotImplementedException();
        return (line.HasSpeaker && line.speakerMdl.name.ToLower() == Keyword);
    }

    /// <summary>
    /// Checks if the elements of choices starts with the -, that indicates a possible choice.
    /// </summary>
    /// <param name="line">Line element.</param>
    /// <returns>Wether the element is a choice or not.</returns>
    private bool IsChoiceStart(string line) => line.Trim().StartsWith(CHOICE_IDENTIFIER);

    /// <summary>
    /// Struct that stores the Choice data, contains the title (choice name) and the result lines (the thing that will be executing after choice was selected).
    /// </summary>
    private struct Choice
    {
        public string title;
        public List<string> resultLines;
        public int startIndex;
        public int endIndex;
    }
}
