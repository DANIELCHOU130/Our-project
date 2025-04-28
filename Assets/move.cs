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
    public PositionDataStorage positionDataStorage;

    void Start()
    {
        transform.position = positions[currentIndex];
    }

    public IEnumerator MoveSteps(int steps)
    {
        if (isMoving) yield break;
        isMoving = true;

        for (int i = 0; i < steps + 1; i++)
        {
            currentIndex = (currentIndex + 1) % positions.Length;
            yield return StartCoroutine(MoveToPosition(positions[currentIndex]));
        }

        isMoving = false;

        // 更新本地 UI
        if (positionDataStorage != null)
        {
            positionDataStorage.UpdatePosition(transform.position);
        }

        // ⚡ 移動結束後，發送自己的座標到伺服器
        if (NetworkClient.Instance != null && !string.IsNullOrEmpty(NetworkClient.Instance.myPlayerName))
        {
            string message = $"{NetworkClient.Instance.myPlayerName},{transform.position.x},{transform.position.y}";
            NetworkClient.Instance.SendMessageToServer(message);
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
