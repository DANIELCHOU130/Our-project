using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class WaitingPanelManager : MonoBehaviour
{
    public TMP_Text statusText;
    public GameObject waitingPanel;

    private int currentGameId;
    private Coroutine checkCoroutine;

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

            yield return new WaitForSeconds(2f);
        }
    }

    public void HandleServerResponse(string msg)
    {
        if (msg.StartsWith("WAITING"))
        {
            string[] parts = msg.Split(',');
            if (parts.Length >= 2)
                statusText.text = $"目前人數: {parts[1]}/4，等待開始...";
        }
        else if (msg == "START_GAME")
        {
            statusText.text = "人數已滿，進入遊戲...";
            StopCoroutine(checkCoroutine);
            Invoke("LoadGameScene", 2f);
        }
    }

    void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    // ===== 資料結構 =====
    [System.Serializable]
    public class GamerIdRequest { public int gamerId; }

    [System.Serializable]
    public class CreateResponse { public string result; public int gameid; }

    [System.Serializable]
    public class JoinRequest { public int gameId; public int gamerId; }
}