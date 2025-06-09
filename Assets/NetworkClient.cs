using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance;

    public string apiBaseUrl = "http://134.208.97.162:5000/it/ESGJOIN/api/game";
    public string myPlayerName = "";

    public Action<string> OnReceiveMessage;       // 接收到 TURN, MOVE, 等等
    public Action<string> OnReceiveCard;          // 接收到 CARD:xxx
    public Action<string> OnAssignedPlayerName;   // 分配到的名稱

    private float pollingInterval = 1.5f;
    private string lastTimestamp = "0"; // 用來記錄最後一筆訊息時間

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(JoinGame());
        StartCoroutine(PollMessages());
    }

    IEnumerator JoinGame()
    {
        UnityWebRequest www = UnityWebRequest.PostWwwForm(apiBaseUrl + "/join", "");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            myPlayerName = www.downloadHandler.text;
            Debug.Log("加入成功，玩家名稱：" + myPlayerName);
            OnAssignedPlayerName?.Invoke(myPlayerName);
        }
        else
        {
            Debug.LogError("加入遊戲失敗: " + www.error);
        }
    }

    public void SendMessageToServer(string message)
    {
        StartCoroutine(SendMessageCoroutine(message));
    }

    IEnumerator SendMessageCoroutine(string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", message);

        UnityWebRequest www = UnityWebRequest.Post(apiBaseUrl + "/message", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("傳送訊息失敗: " + www.error);
        }
    }

    IEnumerator PollMessages()
    {
        while (true)
        {
            string url = $"{apiBaseUrl}/messages?since={lastTimestamp}";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
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
        }
    }

    [Serializable]
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
    }
}
