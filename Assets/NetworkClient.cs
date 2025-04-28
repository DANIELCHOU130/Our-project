using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    public string serverIP = "你的伺服器IP";
    public int serverPort = 5566;

    public Action<string> OnReceiveMessage; // 移動座標訊息
    public Action<string> OnReceiveCard;    // 🔥 新增：卡片資料訊息
    public Action<string> OnAssignedPlayerName;

    public string myPlayerName = "";

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
        ConnectToServer();
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("連線到伺服器成功");
        }
        catch (Exception ex)
        {
            Debug.LogError("連線伺服器失敗: " + ex.Message);
        }
    }

    public void SendMessageToServer(string message)
    {
        if (client == null || !client.Connected) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("傳送資料失敗: " + ex.Message);
        }
    }

    private void ReceiveData()
    {
        try
        {
            while (true)
            {
                if (stream == null) break;

                byte[] buffer = new byte[2048]; // buffer稍微大一點
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("收到資料：" + message);

                if (message.StartsWith("ASSIGN:"))
                {
                    myPlayerName = message.Substring(7);
                    Debug.Log($"取得自己的玩家代號：{myPlayerName}");
                    OnAssignedPlayerName?.Invoke(myPlayerName);
                }
                else if (message.StartsWith("CARD:"))
                {
                    Debug.Log("收到其他玩家的卡片資料！");
                    OnReceiveCard?.Invoke(message.Substring(5)); // 去掉 "CARD:"
                }
                else
                {
                    // 正常的移動資料
                    OnReceiveMessage?.Invoke(message);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("接收資料失敗: " + ex.Message);
        }
    }

    public void Disconnect()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
