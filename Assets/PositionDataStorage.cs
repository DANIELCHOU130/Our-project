using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PositionDataStorage : MonoBehaviour
{
    private readonly Dictionary<Vector3, string> positionData = new Dictionary<Vector3, string>();

    [Header("UI & Components")]
    public TMP_Text textDisplay;
    public CardDisplay cardDisplay;

    private readonly Vector3[] positions = {
        new Vector3(-18.8f, -10f, 0f), new Vector3(-15f, -10f, 0f), new Vector3(-12.5f, -10f, 0f), new Vector3(-10f, -10f, 0f),
        new Vector3(-7.5f, -10f, 0f), new Vector3(-5.1f, -10f, 0f), new Vector3(-2.65f, -10f, 0f), new Vector3(-0.17f, -10f, 0f),
        new Vector3(2.26f, -10f, 0f), new Vector3(4.73f, -10f, 0f), new Vector3(7.13f, -10f, 0f), new Vector3(9.6f, -10f, 0f),
        new Vector3(12.1f, -10f, 0f), new Vector3(14.5f, -10f, 0f), new Vector3(17.75f, -10f, 0f),
        new Vector3(17.75f, -6.5f, 0f), new Vector3(17.75f, -3.7f, 0f), new Vector3(17.75f, -1.22f, 0f),
        new Vector3(17.75f, 1.51f, 0f), new Vector3(17.75f, 4.16f, 0f), new Vector3(17.75f, 6.73f, 0f), new Vector3(17.75f, 9.1f, 0f),
        new Vector3(14.5f, 9.1f, 0f), new Vector3(12.1f, 9.1f, 0f), new Vector3(9.6f, 9.1f, 0f), new Vector3(7.13f, 9.1f, 0f),
        new Vector3(4.73f, 9.1f, 0f), new Vector3(2.26f, 9.1f, 0f), new Vector3(-0.17f, 9.1f, 0f), new Vector3(-2.65f, 9.1f, 0f),
        new Vector3(-5.1f, 9.1f, 0f), new Vector3(-7.5f, 9.1f, 0f), new Vector3(-10f, 9.1f, 0f), new Vector3(-12.5f, 9.1f, 0f),
        new Vector3(-15f, 9.1f, 0f), new Vector3(-18.8f, 9.1f, 0f),
        new Vector3(-18.8f, 6.73f, 0f), new Vector3(-18.8f, 4.16f, 0f), new Vector3(-18.8f, 1.51f, 0f),
        new Vector3(-18.8f, -1.22f, 0f), new Vector3(-18.8f, -3.7f, 0f), new Vector3(-18.8f, -6.5f, 0f)
    };

    private readonly string[] positionValues = {
        "3", "4", "2", "1", "3", "2", "1", "3", "4", "1", "2", "4", "1", "2", "3",
        "4", "2", "1", "3", "2", "1", "3",
        "2", "1", "4", "2", "1", "4", "3", "1", "2", "3", "1", "2", "4", "3",
        "4", "1", "2", "4", "1", "3"
    };

    private readonly Dictionary<string, string> valueToText = new Dictionary<string, string>
    {
        { "1", "S" },
        { "2", "E" },
        { "3", "M" },
        { "4", "G" }
    };

    void Start()
    {
        for (int i = 0; i < positions.Length && i < positionValues.Length; i++)
        {
            positionData[positions[i]] = positionValues[i];
        }
    }

    public void UpdatePosition(Vector3 currentPosition)
    {
        if (positionData.TryGetValue(currentPosition, out string code) && valueToText.TryGetValue(code, out string label))
        {
            textDisplay.text = label;
            cardDisplay.ShowCard();
        }
        else
        {
            textDisplay.text = "¥¼ª¾";
        }
    }
}
