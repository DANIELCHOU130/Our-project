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
    private string apiUrl = "http://134.208.97.162:5000/it/ROOMAPI/api/room/positions";

    void Start()
    {
        StartCoroutine(PollOtherPlayers());
    }

    IEnumerator PollOtherPlayers()
    {
        while (true)
        {
            yield return StartCoroutine(FetchPlayerPositions());
            yield return new WaitForSeconds(1.5f); // 每 1.5 秒輪詢一次
        }
    }

    IEnumerator FetchPlayerPositions()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PlayerPosition[] positions = JsonHelper.FromJson<PlayerPosition>(json);

            foreach (var pos in positions)
            {
                if (pos.playerName == NetworkClient.Instance.myPlayerName) continue;

                foreach (var piece in playerPieces)
                {
                    if (piece.playerName == pos.playerName)
                    {
                        StartCoroutine(MoveSmooth(piece.pieceObject.transform, new Vector3(pos.posX, pos.posY, 0f)));
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("取得其他玩家位置失敗: " + request.error);
        }
    }

    IEnumerator MoveSmooth(Transform piece, Vector3 targetPosition)
    {
        Vector3 startPos = piece.position;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            piece.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.position = targetPosition;
    }

    // ✅ 內嵌 JsonHelper：支援解析 JSON 陣列
    public static class JsonHelper
    {
        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }

        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.Items;
        }
    }
}
