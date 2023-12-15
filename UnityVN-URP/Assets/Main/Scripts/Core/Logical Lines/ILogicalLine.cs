using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicalLine
{
    string Keyword { get; }
    bool Matches(DialogLineModel line);
    IEnumerator Execute(DialogLineModel line);
}
