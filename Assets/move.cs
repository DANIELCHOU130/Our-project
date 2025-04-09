using UnityEngine;
using System.Collections;
using System;

public class move : MonoBehaviour
{
    private Vector3[] positions = {
        new Vector3(-14f, 8.75f, 0f), new Vector3(-10f, 8.75f, 0f), new Vector3(-6f, 8.75f, 0f), new Vector3(-2f, 8.75f, 0f),
        new Vector3(2f, 8.75f, 0f), new Vector3(6f, 8.75f, 0f), new Vector3(10f, 8.75f, 0f), new Vector3(14f, 8.75f, 0f),
        new Vector3(14f, 5f, 0f), new Vector3(14f, 1f, 0f), new Vector3(14f, -3f, 0f), new Vector3(14f, -7f, 0f),
        new Vector3(10f, -7f, 0f), new Vector3(6f, -7f, 0f), new Vector3(2f, -7f, 0f), new Vector3(-2f, -7f, 0f),
        new Vector3(-6f, -7f, 0f), new Vector3(-10f, -7f, 0f), new Vector3(-14f, -7f, 0f), new Vector3(-14f, -3f, 0f),
        new Vector3(-14f, 1f, 0f), new Vector3(-14f, 5f, 0f)
    };

    private int currentIndex = 0;
    private bool isMoving = false;
    public int dicenumber = 0;
    public PositionDataStorage positionDataStorage; // 連結 PositionDataStorage 腳本

    public static event Action<string> OnPositionReached;

    void Start()
    {
        transform.position = positions[currentIndex];
    }

    public IEnumerator MoveSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps + 1; i++) // 保持原本的移動步數
        {
            currentIndex = (currentIndex + 1) % positions.Length;
            yield return StartCoroutine(MoveToPosition(positions[currentIndex]));
        }

        isMoving = false;

        if (positionDataStorage != null)
        {
            positionDataStorage.UpdatePosition(transform.position); // 更新 UI 顯示代號
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float jumpHeight = 1.0f;
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, heightOffset, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
