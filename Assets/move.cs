using UnityEngine;
using System.Collections;
using System;

public class move : MonoBehaviour
{
    private Vector3[] positions = {
        new Vector3(-18.8f, -10f, 0f), new Vector3(-15f, -10f, 0f), new Vector3(-12.5f, -10f, 0f), new Vector3(-10f, -10f, 0f), new Vector3(-7.5f, -10f, 0f), new Vector3(-5.1f, -10f, 0f), new Vector3(-2.65f, -10f, 0f), new Vector3(-0.17f, -10f, 0f), 
        new Vector3(2.26f, -10f, 0f), new Vector3(4.73f, -10f, 0f), new Vector3(7.13f, -10f, 0f), new Vector3(9.6f, -10f, 0f), new Vector3(12.1f, -10f, 0f), new Vector3(14.5f, -10f, 0f), new Vector3(17.75f, -10f, 0f),   //下邊
        
        new Vector3(17.75f, -6.5f, 0f), new Vector3(17.75f, -3.7f, 0f), new Vector3(17.75f, -1.22f, 0f), new Vector3(17.75f, 1.51f, 0f), new Vector3(17.75f, 4.16f, 0f), new Vector3(17.75f, 6.73f, 0f), new Vector3(17.75f, 9.1f, 0f),   //右邊

        new Vector3(14.5f, 9.1f, 0f), new Vector3(12.1f, 9.1f, 0f), new Vector3(9.6f, 9.1f, 0f), new Vector3(7.13f, 9.1f, 0f), new Vector3(4.73f, 9.1f, 0f), new Vector3(2.26f, 9.1f, 0f), new Vector3(-0.17f, 9.1f, 0f),
        new Vector3(-2.65f, 9.1f, 0f), new Vector3(-5.1f, 9.1f, 0f), new Vector3(-7.5f, 9.1f, 0f), new Vector3(-10f, 9.1f, 0f), new Vector3(-12.5f, 9.1f, 0f), new Vector3(-15f, 9.1f, 0f), new Vector3(-18.8f, 9.1f, 0f), //上邊

        new Vector3(-18.8f, 6.73f, 0f), new Vector3(-18.8f, 4.16f, 0f), new Vector3(-18.8f, 1.51f, 0f), new Vector3(-18.8f, -1.22f, 0f), new Vector3(-18.8f, -3.7f, 0f), new Vector3(-18.8f, -6.5f, 0f)   //左邊
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
