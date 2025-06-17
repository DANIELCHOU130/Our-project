<<<<<<< HEAD
﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance;

<<<<<<< HEAD
    public string playerName;
    private string baseUrl = "http://localhost:5000/api/game";
    private DateTime lastCheckTime;

    public event Action<string> OnMessageReceived;
=======
    public string apiBaseUrl = "http://134.208.97.162:5000/it/ESGJOIN/api/game";
    public string myPlayerName = "";
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34

    public Action<string> OnReceiveMessage;       // 接收到 TURN, MOVE, 等等
    public Action<string> OnReceiveCard;          // 接收到 CARD:xxx
    public Action<string> OnAssignedPlayerName;   // 分配到的名稱

    private float pollingInterval = 1.5f;
    private string lastTimestamp = "0"; // 用來記錄最後一筆訊息時間

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(JoinGame());
<<<<<<< HEAD
        StartCoroutine(MessagePollingLoop());
=======
        StartCoroutine(PollMessages());
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    }

    IEnumerator JoinGame()
    {
<<<<<<< HEAD
        UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/join", "");
=======
        UnityWebRequest www = UnityWebRequest.PostWwwForm(apiBaseUrl + "/join", "");
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
<<<<<<< HEAD
            var json = www.downloadHandler.text;
            var data = JsonUtility.FromJson<PlayerJoinResult>(json);
            playerName = data.playerName;
            Debug.Log("分配到代號：" + playerName);
        }
        else
        {
            Debug.LogError("加入遊戲失敗：" + www.error);
=======
            myPlayerName = www.downloadHandler.text;
            Debug.Log("加入成功，玩家名稱：" + myPlayerName);
            OnAssignedPlayerName?.Invoke(myPlayerName);
        }
        else
        {
            Debug.LogError("加入遊戲失敗: " + www.error);
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
        }
    }

    public void SendMessageToServer(string content)
    {
<<<<<<< HEAD
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

=======
        StartCoroutine(SendMessageCoroutine(message));
    }

    IEnumerator SendMessageCoroutine(string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", message);

        UnityWebRequest www = UnityWebRequest.Post(apiBaseUrl + "/message", form);
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
<<<<<<< HEAD
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

=======
            Debug.LogError("傳送訊息失敗: " + www.error);
        }
    }

    IEnumerator PollMessages()
    {
        while (true)
        {
            string url = $"{apiBaseUrl}/messages?since={lastTimestamp}";
            UnityWebRequest www = UnityWebRequest.Get(url);
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
<<<<<<< HEAD
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
=======
                MessageList response = JsonUtility.FromJson<MessageList>(json);

                foreach (var msg in response.messages)
                {
                    lastTimestamp = msg.timestamp;

                    Debug.Log($"接收訊息：{msg.content}");

                    // 分類回傳
                    if (msg.content.StartsWith("CARD:"))
                        OnReceiveCard?.Invoke(msg.content.Substring(5));
                    else
                        OnReceiveMessage?.Invoke(msg.content);
                }
            }
            else
            {
                Debug.LogWarning("輪詢失敗: " + www.error);
            }

            yield return new WaitForSeconds(pollingInterval);
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
        }
    }

    [Serializable]
<<<<<<< HEAD
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
=======
    public class Message
    {
        public string sender;
        public string content;
        public string timestamp;
    }

    [Serializable]
    public class MessageList
    {
        public List<Message> messages;
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    }
}
