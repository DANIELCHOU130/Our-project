using UnityEngine;
using TMPro;
using UnityEngine.Networking;
<<<<<<< HEAD
using System.Collections;
=======
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34

public class WaitingPanelManager : MonoBehaviour
{
    public TMP_Text statusText;
    public GameObject waitingPanel;

    private int currentGameId;
    private Coroutine checkCoroutine;

<<<<<<< HEAD
    private string baseUrl = "http://localhost:5000/api/game";

    // ===== 呼叫創建房間 API =====
    public void CreateGame(int gamerId)
    {
        StartCoroutine(CreateGameCoroutine(gamerId));
    }

    IEnumerator CreateGameCoroutine(int gamerId)
    {
        string json = JsonUtility.ToJson(new GamerIdRequest { gamerId = gamerId });

        UnityWebRequest www = new UnityWebRequest(baseUrl + "/create", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(body);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            CreateResponse res = JsonUtility.FromJson<CreateResponse>(www.downloadHandler.text);
            StartWaiting(res.gameid);
        }
        else
        {
            Debug.LogError("創建遊戲失敗: " + www.error);
            statusText.text = "創建失敗";
        }
    }

    // ===== 呼叫加入房間 API =====
    public void JoinGame(int gameId, int gamerId)
    {
        StartCoroutine(JoinGameCoroutine(gameId, gamerId));
    }

    IEnumerator JoinGameCoroutine(int gameId, int gamerId)
    {
        string url = baseUrl + "/join";
        JoinRequest req = new JoinRequest { gameId = gameId, gamerId = gamerId };
        string json = JsonUtility.ToJson(req);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(body);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            StartWaiting(gameId);
        }
        else
        {
            Debug.LogError("加入房間失敗: " + www.error);
            statusText.text = "加入房間失敗";
        }
    }

    // ===== 啟用等待畫面與輪詢 =====
=======
    //  ROOMAPI的網址,放在it底下
    private string apiBaseUrl = "http://134.208.97.162:5000/it/ROOMAPI/api/room";

>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    public void StartWaiting(int gameId)
    {
        currentGameId = gameId;
        waitingPanel.SetActive(true);
        statusText.text = "等待其他玩家加入中...";
        checkCoroutine = StartCoroutine(CheckLoop());
    }

    IEnumerator CheckLoop()
    {
        while (true)
        {
<<<<<<< HEAD
            UnityWebRequest www = UnityWebRequest.Get(baseUrl + $"/status/{currentGameId}");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string msg = www.downloadHandler.text;
                HandleServerResponse(msg);
            }
            else
            {
                statusText.text = "伺服器錯誤...";
            }

=======
            yield return StartCoroutine(CheckRoomStatus());
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator CheckRoomStatus()
    {
        string url = $"{apiBaseUrl}/status?gameId={currentGameId}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            RoomStatusResponse response = JsonUtility.FromJson<RoomStatusResponse>(request.downloadHandler.text);

            if (response == null)
            {
                statusText.text = "解析伺服器回應失敗";
                yield break;
            }

            if (response.playerCount < 4)
            {
                statusText.text = $"目前人數: {response.playerCount}/4，等待開始...";
            }
            else
            {
                statusText.text = "人數已滿，進入遊戲...";
                StopCoroutine(checkCoroutine);
                Invoke("LoadGameScene", 2f);
            }
        }
        else
        {
<<<<<<< HEAD
            statusText.text = "人數已滿，進入遊戲...";
            StopCoroutine(checkCoroutine);
            Invoke("LoadGameScene", 2f);
=======
            statusText.text = "無法連線至伺服器: " + request.error;
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
        }
    }

    void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

<<<<<<< HEAD
    // ===== 資料結構 =====
    [System.Serializable]
    public class GamerIdRequest { public int gamerId; }

    [System.Serializable]
    public class CreateResponse { public string result; public int gameid; }

    [System.Serializable]
    public class JoinRequest { public int gameId; public int gamerId; }
}
=======
    [System.Serializable]
    public class RoomStatusResponse
    {
        public int gameId;
        public int playerCount;
    }
}
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
