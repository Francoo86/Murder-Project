using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LogicalLineUtils.Conditions;
using static LogicalLineUtils.Expressions;
using static LogicalLineUtils.Encapsulation;

public class LLCondition : ILogicalLine
{
    public string Keyword => "if";
    private const string ELSE = "else";
    private readonly string[] CONTAINERS = new string[] { "(", ")" };

    public IEnumerator Execute(DialogLineModel line)
    {
        string rawCondition = ExtractCondition(line.RawData.Trim());
        bool conditionResult = EvaluateCondition(rawCondition);

        Conversation currentConv = DialogController.Instance.convManager.conversation;
        int currentProgress = DialogController.Instance.convManager.conversationProgress;

        EncapsulationData ifData = RipEncapsulationData(currentConv, currentProgress, false);
        EncapsulationData elseData = new EncapsulationData();

        if(ifData.endingIndex + 1 < currentConv.Count)
        {
            string nextLine = currentConv.GetLines()[ifData.endingIndex + 1].Trim();
            if(nextLine == ELSE)
            {
                elseData = RipEncapsulationData(currentConv, ifData.endingIndex + 1, false);
                ifData.endingIndex = elseData.endingIndex;
            }
        }

        currentConv.SetProgress(ifData.endingIndex);

        EncapsulationData selData = conditionResult ? ifData : elseData;

        if (!selData.IsNull && selData.lines.Count > 0)
        {
            Conversation conv = new Conversation(selData.lines);
            //DialogController.Instance.convManager.conversation.SetProgress(selData.endingIndex);
            DialogController.Instance.convManager.EnqueuePriority(conv);
        }

        yield return null;
    }

    public bool Matches(DialogLineModel line)
    {
        return line.RawData.Trim().StartsWith(Keyword);
    }

    private string ExtractCondition(string line)
    {
        int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
        int endIndex = line.IndexOf(CONTAINERS[1]); 

        //Obtener lo que está adentro del parentesis (condicionales simples plox).
        return line.Substring(startIndex, endIndex - startIndex).Trim();
    }
}
