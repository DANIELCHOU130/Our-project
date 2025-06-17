using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance;

    public string playerName;
    private string baseUrl = "http://localhost:5000/api/game";
    private DateTime lastCheckTime;

    public event Action<string> OnMessageReceived;

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
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
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
                    if (msg.sender != playerName) // 避免收到自己
                    {
                        OnMessageReceived?.Invoke(msg.content);
                        lastCheckTime = msg.timestamp;
                    }
                }
            }
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
