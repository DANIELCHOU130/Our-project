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
        new Vector3(-18.8f, -10f, 0f), new Vector3(-15f, -10f, 0f), new Vector3(-12.5f, -10f, 0f), new Vector3(-10f, -10f, 0f), new Vector3(-7.5f, -10f, 0f), new Vector3(-5.1f, -10f, 0f), new Vector3(-2.65f, -10f, 0f), new Vector3(-0.17f, -10f, 0f),
        new Vector3(2.26f, -10f, 0f), new Vector3(4.73f, -10f, 0f), new Vector3(7.13f, -10f, 0f), new Vector3(9.6f, -10f, 0f), new Vector3(12.1f, -10f, 0f), new Vector3(14.5f, -10f, 0f), new Vector3(17.75f, -10f, 0f),   //下邊
        
        new Vector3(17.75f, -6.5f, 0f), new Vector3(17.75f, -3.7f, 0f), new Vector3(17.75f, -1.22f, 0f), new Vector3(17.75f, 1.51f, 0f), new Vector3(17.75f, 4.16f, 0f), new Vector3(17.75f, 6.73f, 0f), new Vector3(17.75f, 9.1f, 0f),   //右邊

        new Vector3(14.5f, 9.1f, 0f), new Vector3(12.1f, 9.1f, 0f), new Vector3(9.6f, 9.1f, 0f), new Vector3(7.13f, 9.1f, 0f), new Vector3(4.73f, 9.1f, 0f), new Vector3(2.26f, 9.1f, 0f), new Vector3(-0.17f, 9.1f, 0f),
        new Vector3(-2.65f, 9.1f, 0f), new Vector3(-5.1f, 9.1f, 0f), new Vector3(-7.5f, 9.1f, 0f), new Vector3(-10f, 9.1f, 0f), new Vector3(-12.5f, 9.1f, 0f), new Vector3(-15f, 9.1f, 0f), new Vector3(-18.8f, 9.1f, 0f), //上邊

        new Vector3(-18.8f, 6.73f, 0f), new Vector3(-18.8f, 4.16f, 0f), new Vector3(-18.8f, 1.51f, 0f), new Vector3(-18.8f, -1.22f, 0f), new Vector3(-18.8f, -3.7f, 0f), new Vector3(-18.8f, -6.5f, 0f)   //左邊
    };

    private string[] positionValues = { 
        "3", "4", "2", "1", "3", "2", "1", "3", "4", "1", "2", "4", "1", "2", "3", //下邊
        "4", "2", "1", "3", "2", "1", "3",
        "2", "1", "4", "2", "1", "4", "3", "1", "2", "3", "1", "2", "4", "3",
        "4", "1", "2", "4", "1", "3"
    };

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
