using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that represents logical elements on the dialog file, like choices, conditions, inputs and operators.
/// </summary>
public interface ILogicalLine
{
    /// <summary>
    /// Keyword associated to the logical line.
    /// </summary>
    string Keyword { get; }
    /// <summary>
    /// Check if some raw line matches with specified criteria of the logical line.
    /// </summary>
    /// <param name="line"></param>
    /// <returns>Wether the match has passed or not.</returns>
    bool Matches(DialogLineModel line);
    /// <summary>
    /// Executes the line as it were interpreted.
    /// </summary>
    /// <param name="line">Runs the logical line.</param>
    /// <returns>The IEnumerator to be yielded in ConversationManager.</returns>
    IEnumerator Execute(DialogLineModel line);
}
