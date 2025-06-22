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
    public string myPlayerName => playerName;

    private const string baseUrl = "http://localhost:5000/api/game";
    private DateTime lastCheckTime;

    public event Action<string> OnMessageReceived;
    public event Action<string> OnReceiveCard;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(JoinGame());
        StartCoroutine(MessagePollingLoop());
    }

    private IEnumerator JoinGame()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{baseUrl}/join", ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var data = JsonUtility.FromJson<PlayerJoinResult>(www.downloadHandler.text);
                playerName = data.playerName;
                Debug.Log("分配到代號：" + playerName);
            }
            else
            {
                Debug.LogError("加入遊戲失敗：" + www.error);
            }
        }
    }

    public void SendMessageToServer(string content)
    {
        StartCoroutine(SendMessageCoroutine(content));
    }

    private IEnumerator SendMessageCoroutine(string content)
    {
        GameMessage msg = new GameMessage { sender = playerName, content = content };
        string json = JsonUtility.ToJson(msg);

        using (UnityWebRequest www = new UnityWebRequest($"{baseUrl}/message", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("送出訊息失敗：" + www.error);
            }
        }
    }

    private IEnumerator MessagePollingLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            string sinceParam = Uri.EscapeDataString(lastCheckTime.ToString("o"));

            using (UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}/messages?since={sinceParam}"))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var wrapper = JsonUtility.FromJson<GameMessageList>("{\"messages\":" + www.downloadHandler.text + "}");
                    foreach (var msg in wrapper.messages)
                    {
                        if (msg.sender != playerName)
                        {
                            if (msg.content.StartsWith("CARD:"))
                                OnReceiveCard?.Invoke(msg.content.Substring("CARD:".Length));
                            else
                                OnMessageReceived?.Invoke(msg.content);

                            lastCheckTime = msg.timestamp;
                        }
                    }
                }
            }
        }
    }

    public void CreateRoom()
    {
        StartCoroutine(CreateRoomCoroutine());
    }

    private IEnumerator CreateRoomCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{baseUrl}/create", ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("房間建立成功：" + www.downloadHandler.text);
            else
                Debug.LogError("房間建立失敗：" + www.error);
        }
    }

    public void JoinRoom(string roomId)
    {
        StartCoroutine(JoinRoomCoroutine(roomId));
    }

    private IEnumerator JoinRoomCoroutine(string roomId)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{baseUrl}/join/{roomId}", ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log($"加入房間成功：房號 {roomId}");
            else
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
