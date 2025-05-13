using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkClient2 : MonoBehaviour
{
    public static NetworkClient2 Instance2;

    private TcpClient client2;
    private NetworkStream stream2;
    private Thread receiveThread2;

    public string serverIP2 = "134.208.97.162"; 
    public int serverPort2 = 5566;

    public Action<string> OnReceiveMessage2; // 收到其他玩家移動資料
    public Action<string> OnAssignedPlayerName2; // 收到自己代號時觸發

    public string myPlayerName2 = ""; // 儲存自己被分配的代號

    void Awake()
    {
        if (Instance2 == null)
        {
            Instance2 = this;
            DontDestroyOnLoad(gameObject); // 不要被切場景破壞
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ConnectToServer2();
    }

    void OnApplicationQuit()
    {
        Disconnect2();
    }

    public void ConnectToServer2()
    {
        try
        {
            client2 = new TcpClient();
            client2.Connect(serverIP2, serverPort2);
            stream2 = client2.GetStream();

            receiveThread2 = new Thread(ReceiveData2);
            receiveThread2.IsBackground = true;
            receiveThread2.Start();

            Debug.Log("連線到伺服器成功");
        }
        catch (Exception ex)
        {
            Debug.LogError("連線伺服器失敗: " + ex.Message);
        }
    }

    public void SendMessageToServer2(string message2)
    {
        if (client2 == null || !client2.Connected) return;

        try
        {
            byte[] data2 = Encoding.UTF8.GetBytes(message2);
            stream2.Write(data2, 0, data2.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("傳送資料失敗: " + ex.Message);
        }
    }

    private void ReceiveData2()
    {
        try
        {
            while (true)
            {
                if (stream2 == null) break;

                byte[] buffer2 = new byte[1024];
                int bytesRead2 = stream2.Read(buffer2, 0, buffer2.Length);
                if (bytesRead2 == 0) break;

                string message2 = Encoding.UTF8.GetString(buffer2, 0, bytesRead2);
                Debug.Log("收到資料：" + message2);

                if (message2.StartsWith("ASSIGN:"))
                {
                    myPlayerName2 = message2.Substring(7);
                    Debug.Log($"取得自己的玩家代號：{myPlayerName2}");
                    OnAssignedPlayerName2?.Invoke(myPlayerName2);
                }
                else
                {
                    OnReceiveMessage2?.Invoke(message2);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("接收資料失敗: " + ex.Message);
        }
    }

    public void Disconnect2()
    {
        if (receiveThread2 != null) receiveThread2.Abort();
        if (stream2 != null) stream2.Close();
        if (client2 != null) client2.Close();
    }
}

