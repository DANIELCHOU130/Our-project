using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OtherPlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPiece
    {
        public string playerName;
        public GameObject pieceObject;
    }

    [System.Serializable]
    public class PlayerPosition
    {
        public string playerName;
        public float posX;
        public float posY;
    }

    public List<PlayerPiece> playerPieces = new List<PlayerPiece>();
    private const string apiUrl = "http://134.208.97.162:5000/it/ROOMAPI/api/room/positions";

    private void Start()
    {
        StartCoroutine(PollOtherPlayers());
    }

    private IEnumerator PollOtherPlayers()
    {
        while (true)
        {
            yield return FetchPlayerPositions();
            yield return new WaitForSeconds(1.5f);
        }
    }

    private IEnumerator FetchPlayerPositions()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            var positions = JsonHelper.FromJson<PlayerPosition>(json);

            foreach (var pos in positions)
            {
                if (pos.playerName == NetworkClient.Instance.myPlayerName) continue;

                var piece = playerPieces.Find(p => p.playerName == pos.playerName);
                if (piece != null)
                {
                    StartCoroutine(MoveSmooth(piece.pieceObject.transform, new Vector3(pos.posX, pos.posY, 0f)));
                }
            }
        }
        else
        {
            Debug.LogError("取得其他玩家位置失敗: " + request.error);
        }
    }

    private IEnumerator MoveSmooth(Transform piece, Vector3 targetPosition)
    {
        Vector3 startPos = piece.position;
        float elapsedTime = 0f;
        const float duration = 0.5f;

        while (elapsedTime < duration)
        {
            piece.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.position = targetPosition;
    }

    public static class JsonHelper
    {
        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }

        public static T[] FromJson<T>(string json)
        {
            string wrappedJson = "{\"Items\":" + json + "}";
            return JsonUtility.FromJson<Wrapper<T>>(wrappedJson).Items;
        }
    }
}
