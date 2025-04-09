using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PositionDataStorage : MonoBehaviour
{
    private Dictionary<Vector3, string> positionData = new Dictionary<Vector3, string>();

    public TMP_Text textDisplay; // 使用 TMP_Text 而非 Text
    public CardDisplay cardDisplay; // 引用 CardDisplay 類別

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
            textDisplay.text = valueToText[code]; // 顯示對應文字

            // 呼叫 ShowCard 方法，將顯示的文字作為參數
            cardDisplay.ShowCard(); // 更新顯示的卡片面板
        }
        else
        {
            textDisplay.text = "未知"; // 若無匹配則顯示未知
        }
    }
}
