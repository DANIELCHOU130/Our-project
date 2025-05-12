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

            Debug.Log("�w�s�u��p���C�����A��");
        }
        catch (Exception ex)
        {
            Debug.LogError("�s�u����: " + ex.Message);
        }
    }

    public void SendCommand(string command)
    {
        if (client == null || !client.Connected) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
            Debug.Log("�e�X���O: " + command);
        }
        catch (Exception ex)
        {
            Debug.LogError("�ǰe���O����: " + ex.Message);
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
                Debug.Log("���A���^��: " + response);

                OnServerResponse?.Invoke(response);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("������ƥ���: " + ex.Message);
        }
    }

    public void Disconnect()
    {
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();
    }

    // �H�U�O�T�ӫ��s�M�Ϊ��I�s��k�G

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
