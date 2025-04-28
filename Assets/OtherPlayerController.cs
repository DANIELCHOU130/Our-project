using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPiece
    {
        public string playerName;      // 玩家代號 (如 Player2, Player3, Player4)
        public GameObject pieceObject; // 對應的棋子物件
    }

    public List<PlayerPiece> playerPieces = new List<PlayerPiece>();

    void Start()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnReceiveMessage += OnReceiveNetworkMessage;
        }
    }

    void OnDestroy()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnReceiveMessage -= OnReceiveNetworkMessage;
        }
    }

    private void OnReceiveNetworkMessage(string message)
    {
        // 預期資料格式: "Player2,10,8.75"
        string[] parts = message.Split(',');
        if (parts.Length != 3) return;

        string playerName = parts[0];
        if (float.TryParse(parts[1], out float posX) && float.TryParse(parts[2], out float posY))
        {
            // 避免自己收到自己
            if (playerName == NetworkClient.Instance.myPlayerName)
                return;

            MovePlayerPiece(playerName, new Vector3(posX, posY, 0f));
        }
    }

    private void MovePlayerPiece(string playerName, Vector3 targetPosition)
    {
        foreach (var playerPiece in playerPieces)
        {
            if (playerPiece.playerName == playerName)
            {
                StartCoroutine(MoveSmooth(playerPiece.pieceObject.transform, targetPosition));
                break;
            }
        }
    }

    private System.Collections.IEnumerator MoveSmooth(Transform piece, Vector3 targetPosition)
    {
        Vector3 startPos = piece.position;
        float elapsedTime = 0f;
        float moveDuration = 0.5f; // 移動花費的時間

        while (elapsedTime < moveDuration)
        {
            piece.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.position = targetPosition;
    }
}
