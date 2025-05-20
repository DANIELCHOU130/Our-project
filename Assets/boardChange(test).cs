using System.Collections.Generic;
using UnityEngine;

public class BoardGridGenerator : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer; // �I���� SpriteRenderer
    public Vector2 rectangleSize = new Vector2(2f, 1f); // ����Τj�p (�e x ��)
    public Vector2 squareSize; // ����Τj�p (�� = rectangleSize.x)
    private List<Vector2> gridPositions; // �x�s��l���ߪ��y��

    void Start()
    {
        if (backgroundRenderer == null)
        {
            Debug.LogError("�Ы����I���Ϥ��� SpriteRenderer�I");
            return;
        }

        // �]�m����Ϊ��j�p���x�Ϊ��e��
        squareSize = new Vector2(rectangleSize.x, rectangleSize.x);

        // �}�l�ͦ���l
        GenerateGrid();
    }

    void GenerateGrid()
    {
        gridPositions = new List<Vector2>();

        // ����I������� (�@�ɧ���)
        Bounds bounds = backgroundRenderer.bounds;

        // �_�l�I�P�I���j�p
        float startX = bounds.min.x; // ���U�� X
        float startY = bounds.max.y; // ���W�� Y
        float boardWidth = bounds.size.x; // �I���e��
        float boardHeight = bounds.size.y; // �I������

        // �p��|���䪺��l�ƶq
        int horizontalCount = Mathf.FloorToInt((boardWidth - 2 * squareSize.x) / rectangleSize.x); // ���k����������l���ƶq
        int verticalCount = Mathf.FloorToInt((boardHeight - 2 * squareSize.y) / rectangleSize.y); // �W�U�����ݪ���l���ƶq

        // �|�Ө���
        AddSquare(startX, startY); // ���W��
        AddSquare(startX + boardWidth - squareSize.x, startY); // �k�W��
        AddSquare(startX, startY - boardHeight + squareSize.y); // ���U��
        AddSquare(startX + boardWidth - squareSize.x, startY - boardHeight + squareSize.y); // �k�U��

        // �W�����
        for (int i = 0; i < horizontalCount; i++)
        {
            float posX = startX + squareSize.x + i * rectangleSize.x + rectangleSize.x / 2;
            float posY = startY - rectangleSize.y / 2;
            AddRectangle(posX, posY, false); // �ݪ������
        }

        // �U������
        for (int i = 0; i < horizontalCount; i++)
        {
            float posX = startX + squareSize.x + i * rectangleSize.x + rectangleSize.x / 2;
            float posY = startY - boardHeight + rectangleSize.y / 2;
            AddRectangle(posX, posY, false); // �ݪ������
        }

        // ��������
        for (int i = 0; i < verticalCount; i++)
        {
            float posX = startX + rectangleSize.y / 2;
            float posY = startY - squareSize.y - i * rectangleSize.y - rectangleSize.y / 2;
            AddRectangle(posX, posY, true); // ���������
        }

        // �k������
        for (int i = 0; i < verticalCount; i++)
        {
            float posX = startX + boardWidth - rectangleSize.y / 2;
            float posY = startY - squareSize.y - i * rectangleSize.y - rectangleSize.y / 2;
            AddRectangle(posX, posY, true); // ���������
        }

        // Debug�G�C�X�Ҧ��p�⪺��l���߮y��
        foreach (Vector2 pos in gridPositions)
        {
            Debug.Log($"��l���߮y��: {pos}");
        }
    }

    // �K�[����ή�l
    void AddSquare(float centerX, float centerY)
    {
        Vector2 squarePosition = new Vector2(centerX + squareSize.x / 2, centerY - squareSize.y / 2);
        gridPositions.Add(squarePosition);

        // ���եi���� (����N�����)
        Debug.DrawLine(
            new Vector3(centerX, centerY, 0),
            new Vector3(centerX + squareSize.x, centerY - squareSize.y, 0),
            Color.red,
            10f
        );
    }

    // �K�[����ή�l
    void AddRectangle(float centerX, float centerY, bool isHorizontal)
    {
        float width = isHorizontal ? rectangleSize.x : rectangleSize.y;
        float height = isHorizontal ? rectangleSize.y : rectangleSize.x;

        Vector2 rectPosition = new Vector2(centerX, centerY);
        gridPositions.Add(rectPosition);

        // ���եi���� (���N������)
        Debug.DrawLine(
            new Vector3(centerX - width / 2, centerY - height / 2, 0),
            new Vector3(centerX + width / 2, centerY + height / 2, 0),
            Color.green,
            10f
        );
    }
}
