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
    private bool isRunning = false;

    public string serverIP = "134.208.97.162";
    public int serverPort = 5000;

    public Action<string> OnReceiveMessage;
    public Action<string> OnReceiveCard;
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

            isRunning = true;
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
            while (isRunning)
            {
                if (stream == null || !stream.CanRead) break;

                byte[] buffer = new byte[2048];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("收到資料：" + message);

                if (message.StartsWith("ASSIGN:"))
                {
                    myPlayerName = message.Substring(7);
                    OnAssignedPlayerName?.Invoke(myPlayerName);
                }
                else if (message.StartsWith("CARD:"))
                {
                    OnReceiveCard?.Invoke(message.Substring(5));
                }
                else
                {
                    OnReceiveMessage?.Invoke(message);
                }
            }
        }
        catch (Exception ex)
        {
            if (isRunning)
                Debug.LogError("接收資料失敗: " + ex.Message);
        }
    }

    public void Disconnect()
    {
        isRunning = false;

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join(); // 等待執行緒自然結束

        stream?.Close();
        client?.Close();
    }
}
