using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance;

    public string playerName;
    public string myPlayerName => playerName; // 外部可取得
    private string baseUrl = "http://localhost:5000/api/game";
    private DateTime lastCheckTime;

    public event Action<string> OnMessageReceived;
    public event Action<string> OnReceiveCard; // 🔥 新增：小面板用

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(JoinGame());
        StartCoroutine(MessagePollingLoop());
    }

    IEnumerator JoinGame()
    {
        UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/join", "");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var json = www.downloadHandler.text;
            var data = JsonUtility.FromJson<PlayerJoinResult>(json);
            playerName = data.playerName;
            Debug.Log("分配到代號：" + playerName);
        }
        else
        {
            Debug.LogError("加入遊戲失敗：" + www.error);
        }
    }

    public void SendMessageToServer(string content)
    {
        StartCoroutine(SendMessageCoroutine(content));
    }

    IEnumerator SendMessageCoroutine(string content)
    {
        GameMessage msg = new GameMessage { sender = playerName, content = content };
        string json = JsonUtility.ToJson(msg);

        UnityWebRequest www = new UnityWebRequest($"{baseUrl}/message", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("送出訊息失敗：" + www.error);
        }
    }

    IEnumerator MessagePollingLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            string sinceParam = Uri.EscapeDataString(lastCheckTime.ToString("o"));
            UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}/messages?since={sinceParam}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                GameMessageList wrapper = JsonUtility.FromJson<GameMessageList>("{\"messages\":" + json + "}");
                foreach (var msg in wrapper.messages)
                {
                    if (msg.sender != playerName)
                    {
                        // 判斷是否為 CARD 訊息
                        if (msg.content.StartsWith("CARD:"))
                        {
                            string cardData = msg.content.Substring("CARD:".Length);
                            OnReceiveCard?.Invoke(cardData);
                        }
                        else
                        {
                            OnMessageReceived?.Invoke(msg.content);
                        }
                        lastCheckTime = msg.timestamp;
                    }
                }
            }
        }
    }

    // 🔧 呼叫 API 建立房間（無需額外參數）
    public void CreateRoom()
    {
        StartCoroutine(CreateRoomCoroutine());
    }

    private IEnumerator CreateRoomCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/create", "");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("房間建立成功：" + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("房間建立失敗：" + www.error);
        }
    }

    // 🔧 呼叫 API 加入房間（你需要在 UI 中輸入房號）
    public void JoinRoom(string roomId)
    {
        StartCoroutine(JoinRoomCoroutine(roomId));
    }

    private IEnumerator JoinRoomCoroutine(string roomId)
    {
        UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/join/{roomId}", "");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"加入房間成功：房號 {roomId}");
        }
        else
        {
            Debug.LogError($"加入房間失敗：{www.error}");
        }
    }

    [Serializable]
    public class PlayerJoinResult
    {
        public string playerName;
    }

    [Serializable]
    public class GameMessage
    {
        public string sender;
        public string content;
        public DateTime timestamp;
    }

    [Serializable]
    public class GameMessageList
    {
        public List<GameMessage> messages;
    }
}
