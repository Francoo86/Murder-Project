using UnityEngine;

/// <summary>
/// ScriptableObject to configurate the starting and ending files of the VN.
/// </summary>
[CreateAssetMenu(fileName = "Visual Novel Configuration", menuName = "Dialog System/Visual Novel Configuration Asset")]

public class VisualNovelSO : ScriptableObject
{
    public TextAsset startingFile;
}
