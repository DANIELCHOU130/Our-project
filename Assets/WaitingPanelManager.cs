using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class WaitingPanelManager : MonoBehaviour
{
    public TMP_Text statusText;
    public GameObject waitingPanel;

    private int currentGameId;
    private Coroutine checkCoroutine;

    //  ROOMAPI的網址,放在it底下
    private string apiBaseUrl = "http://134.208.97.162:5000/it/ROOMAPI/api/room";

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
            yield return StartCoroutine(CheckRoomStatus());
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
            statusText.text = "無法連線至伺服器: " + request.error;
        }
    }

    void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    [System.Serializable]
    public class RoomStatusResponse
    {
        public int gameId;
        public int playerCount;
    }
}
