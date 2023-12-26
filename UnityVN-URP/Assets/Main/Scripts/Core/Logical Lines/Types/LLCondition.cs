using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
using static LogicalLineUtils.Conditions;
//using static LogicalLineUtils.Expressions;
using static LogicalLineUtils.Encapsulation;


/// <summary>
/// Class that handles the if-else clauses in the dialog file.
/// </summary>
public class LLCondition : ILogicalLine
{
    public string Keyword => "if";
    private const string ELSE = "else";
    private readonly string[] CONTAINERS = new string[] { "(", ")" };

    /// <summary>
    /// Runs the if conditional and its lines inside it, also if it has an else condition it will do the same.
    /// </summary>
    /// <param name="line"></param>
    /// <returns>The IEnumerator to be yielded.</returns>
    public IEnumerator Execute(DialogLineModel line)
    {
        string rawCondition = ExtractCondition(line.RawData.Trim());
        bool conditionResult = EvaluateCondition(rawCondition);

        Conversation currentConv = DialogController.Instance.convManager.conversation;
        int currentProgress = DialogController.Instance.convManager.conversationProgress;

        EncapsulationData ifData = RipEncapsulationData(currentConv, currentProgress, false, parentStartingIndex: currentConv.fileStartIndex);
        EncapsulationData elseData = new EncapsulationData();

        if(ifData.endingIndex + 1 < currentConv.Count)
        {
            string nextLine = currentConv.GetLines()[ifData.endingIndex + 1].Trim();
            if(nextLine == ELSE)
            {
                elseData = RipEncapsulationData(currentConv, ifData.endingIndex + 1, false, parentStartingIndex: currentConv.fileStartIndex);
                //ifData.endingIndex = elseData.endingIndex;
            }
        }

        currentConv.SetProgress(elseData.IsNull? ifData.endingIndex : elseData.endingIndex);

        EncapsulationData selData = conditionResult ? ifData : elseData;

        if (!selData.IsNull && selData.lines.Count > 0)
        {
            selData.startingIndex += 2;
            selData.endingIndex -= 1;   

            Conversation conv = new Conversation(selData.lines, file: currentConv.file, fileStartIndex: selData.startingIndex, fileEndIndex: selData.endingIndex);
            //DialogController.Instance.convManager.conversation.SetProgress(selData.endingIndex);
            DialogController.Instance.convManager.EnqueuePriority(conv);
        }

        yield return null;
    }

    /// <summary>
    /// Checks if the line matches with 'if' keyword.
    /// </summary>
    /// <param name="line">Dialog line.</param>
    /// <returns>Wether the line matches with the conditional keyword.</returns>
    public bool Matches(DialogLineModel line)
    {
        return line.RawData.Trim().StartsWith(Keyword);
    }

    /// <summary>
    /// Extracts the full condition inside the if.
    /// </summary>
    /// <param name="line">The raw line with the condition.</param>
    /// <returns>The condition without the parenthesis and the if.</returns>
    private string ExtractCondition(string line)
    {
        int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
        int endIndex = line.IndexOf(CONTAINERS[1]); 

        //Obtener lo que está adentro del parentesis (condicionales simples plox).
        return line.Substring(startIndex, endIndex - startIndex).Trim();
    }
}
