using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PositionDataStorage : MonoBehaviour
{
    private Dictionary<Vector3, string> positionData = new Dictionary<Vector3, string>();

    public TMP_Text textDisplay; // �ϥ� TMP_Text �ӫD Text
    public CardDisplay cardDisplay; // �ޥ� CardDisplay ���O

    private Vector3[] positions = {
        new Vector3(-14f, 8.75f, 0f), new Vector3(-10f, 8.75f, 0f), new Vector3(-6f, 8.75f, 0f), new Vector3(-2f, 8.75f, 0f),
        new Vector3(2f, 8.75f, 0f), new Vector3(6f, 8.75f, 0f), new Vector3(10f, 8.75f, 0f), new Vector3(14f, 8.75f, 0f),
        new Vector3(14f, 5f, 0f), new Vector3(14f, 1f, 0f), new Vector3(14f, -3f, 0f), new Vector3(14f, -7f, 0f),
        new Vector3(10f, -7f, 0f), new Vector3(6f, -7f, 0f), new Vector3(2f, -7f, 0f), new Vector3(-2f, -7f, 0f),
        new Vector3(-6f, -7f, 0f), new Vector3(-10f, -7f, 0f), new Vector3(-14f, -7f, 0f), new Vector3(-14f, -3f, 0f),
        new Vector3(-14f, 1f, 0f), new Vector3(-14f, 5f, 0f)
    };

    private string[] positionValues = { "1", "2", "3", "4", "2", "3", "1", "4", "2", "4", "1", "3", "2", "3", "1", "4", "2", "3", "1", "4", "2", "4" };

    private Dictionary<string, string> valueToText = new Dictionary<string, string>
    {
        { "1", "S" },
        { "2", "E" },
        { "3", "M" },
        { "4", "G" }
    };

    void Start()
    {
        StorePositionData();
    }

    void StorePositionData()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positionData[positions[i]] = positionValues[i];
        }
    }

    public void UpdatePosition(Vector3 currentPosition)
    {
        if (positionData.ContainsKey(currentPosition))
        {
            string code = positionData[currentPosition];
            textDisplay.text = valueToText[code]; // ��ܹ�����r

            // �I�s ShowCard ��k�A�N��ܪ���r�@���Ѽ�
            cardDisplay.ShowCard(); // ��s��ܪ��d�����O
        }
        else
        {
            textDisplay.text = "����"; // �Y�L�ǰt�h��ܥ���
        }
    }
}
