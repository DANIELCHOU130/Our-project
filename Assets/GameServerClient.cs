using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GameServerClient : MonoBehaviour
{
    public string serverIP = "134.208.97.162";
    public int serverPort = 5567;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    public Action<string> OnServerResponse;

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

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("已連線到小型遊戲伺服器");
        }
        catch (Exception ex)
        {
            Debug.LogError("連線失敗: " + ex.Message);
        }
    }

    public void SendCommand(string command)
    {
        if (client == null || !client.Connected) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
            Debug.Log("送出指令: " + command);
        }
        catch (Exception ex)
        {
            Debug.LogError("傳送指令失敗: " + ex.Message);
        }
    }

    private void ReceiveLoop()
    {
        try
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Debug.Log("伺服器回應: " + response);

                OnServerResponse?.Invoke(response);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("接收資料失敗: " + ex.Message);
        }
    }

    public void Disconnect()
    {
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();
    }

    // 以下是三個按鈕專用的呼叫方法：

    public void RequestCreateGame(int gamerId)
    {
        SendCommand($"CREATE,{gamerId}");
    }

    public void RequestJoinGame(int gameId, int gamerId)
    {
        SendCommand($"JOIN,{gameId},{gamerId}");
    }

    public void RequestListGames()
    {
        SendCommand("LIST");
    }
}
