using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPiece
    {
        public string playerName;      // ���a�N�� (�p Player2, Player3, Player4)
        public GameObject pieceObject; // �������Ѥl����
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
        // �w����Ʈ榡: "Player2,10,8.75"
        string[] parts = message.Split(',');
        if (parts.Length != 3) return;

        string playerName = parts[0];
        if (float.TryParse(parts[1], out float posX) && float.TryParse(parts[2], out float posY))
        {
            // �קK�ۤv����ۤv
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
        float moveDuration = 0.5f; // ���ʪ�O���ɶ�

        while (elapsedTime < moveDuration)
        {
            piece.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.position = targetPosition;
    }
}
