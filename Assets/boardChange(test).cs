using System.Collections.Generic;
using UnityEngine;

public class BoardGridGenerator : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer; // 背景的 SpriteRenderer
    public Vector2 rectangleSize = new Vector2(2f, 1f); // 長方形大小 (寬 x 高)
    public Vector2 squareSize; // 正方形大小 (長 = rectangleSize.x)
    private List<Vector2> gridPositions; // 儲存格子中心的座標

    void Start()
    {
        if (backgroundRenderer == null)
        {
            Debug.LogError("請指派背景圖片的 SpriteRenderer！");
            return;
        }

        // 設置正方形的大小為矩形的寬度
        squareSize = new Vector2(rectangleSize.x, rectangleSize.x);

        // 開始生成格子
        GenerateGrid();
    }

    void GenerateGrid()
    {
        gridPositions = new List<Vector2>();

        // 獲取背景的邊界 (世界坐標)
        Bounds bounds = backgroundRenderer.bounds;

        // 起始點與背景大小
        float startX = bounds.min.x; // 左下角 X
        float startY = bounds.max.y; // 左上角 Y
        float boardWidth = bounds.size.x; // 背景寬度
        float boardHeight = bounds.size.y; // 背景高度

        // 計算四條邊的格子數量
        int horizontalCount = Mathf.FloorToInt((boardWidth - 2 * squareSize.x) / rectangleSize.x); // 左右兩條邊水平格子的數量
        int verticalCount = Mathf.FloorToInt((boardHeight - 2 * squareSize.y) / rectangleSize.y); // 上下兩條邊豎直格子的數量

        // 四個角落
        AddSquare(startX, startY); // 左上角
        AddSquare(startX + boardWidth - squareSize.x, startY); // 右上角
        AddSquare(startX, startY - boardHeight + squareSize.y); // 左下角
        AddSquare(startX + boardWidth - squareSize.x, startY - boardHeight + squareSize.y); // 右下角

        // 上長方形
        for (int i = 0; i < horizontalCount; i++)
        {
            float posX = startX + squareSize.x + i * rectangleSize.x + rectangleSize.x / 2;
            float posY = startY - rectangleSize.y / 2;
            AddRectangle(posX, posY, false); // 豎直長方形
        }

        // 下邊長方形
        for (int i = 0; i < horizontalCount; i++)
        {
            float posX = startX + squareSize.x + i * rectangleSize.x + rectangleSize.x / 2;
            float posY = startY - boardHeight + rectangleSize.y / 2;
            AddRectangle(posX, posY, false); // 豎直長方形
        }

        // 左邊長方形
        for (int i = 0; i < verticalCount; i++)
        {
            float posX = startX + rectangleSize.y / 2;
            float posY = startY - squareSize.y - i * rectangleSize.y - rectangleSize.y / 2;
            AddRectangle(posX, posY, true); // 水平長方形
        }

        // 右邊長方形
        for (int i = 0; i < verticalCount; i++)
        {
            float posX = startX + boardWidth - rectangleSize.y / 2;
            float posY = startY - squareSize.y - i * rectangleSize.y - rectangleSize.y / 2;
            AddRectangle(posX, posY, true); // 水平長方形
        }

        // Debug：列出所有計算的格子中心座標
        foreach (Vector2 pos in gridPositions)
        {
            Debug.Log($"格子中心座標: {pos}");
        }
    }

    // 添加正方形格子
    void AddSquare(float centerX, float centerY)
    {
        Vector2 squarePosition = new Vector2(centerX + squareSize.x / 2, centerY - squareSize.y / 2);
        gridPositions.Add(squarePosition);

        // 測試可視化 (紅色代表正方形)
        Debug.DrawLine(
            new Vector3(centerX, centerY, 0),
            new Vector3(centerX + squareSize.x, centerY - squareSize.y, 0),
            Color.red,
            10f
        );
    }

    // 添加長方形格子
    void AddRectangle(float centerX, float centerY, bool isHorizontal)
    {
        float width = isHorizontal ? rectangleSize.x : rectangleSize.y;
        float height = isHorizontal ? rectangleSize.y : rectangleSize.x;

        Vector2 rectPosition = new Vector2(centerX, centerY);
        gridPositions.Add(rectPosition);

        // 測試可視化 (綠色代表長方形)
        Debug.DrawLine(
            new Vector3(centerX - width / 2, centerY - height / 2, 0),
            new Vector3(centerX + width / 2, centerY + height / 2, 0),
            Color.green,
            10f
        );
    }
}
