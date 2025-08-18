using UnityEngine;

[CreateAssetMenu(fileName = "EndingData", menuName = "Game/EndingData")]
public class EndingData : ScriptableObject
{
    public string endingTitle;
    [TextArea(3, 10)]
    public string[] endingLines;
    public AudioClip bgmClip;
}
