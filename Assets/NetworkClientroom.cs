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

    public Action<string> OnReceiveMessage2; // �����L���a���ʸ��
    public Action<string> OnAssignedPlayerName2; // ����ۤv�N����Ĳ�o

    public string myPlayerName2 = ""; // �x�s�ۤv�Q���t���N��

    void Awake()
    {
        if (Instance2 == null)
        {
            Instance2 = this;
            DontDestroyOnLoad(gameObject); // ���n�Q�������}�a
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

            Debug.Log("�s�u����A�����\");
        }
        catch (Exception ex)
        {
            Debug.LogError("�s�u���A������: " + ex.Message);
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
            Debug.LogError("�ǰe��ƥ���: " + ex.Message);
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
                Debug.Log("�����ơG" + message2);

                if (message2.StartsWith("ASSIGN:"))
                {
                    myPlayerName2 = message2.Substring(7);
                    Debug.Log($"���o�ۤv�����a�N���G{myPlayerName2}");
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
            Debug.LogError("������ƥ���: " + ex.Message);
        }
    }

    public void Disconnect2()
    {
        if (receiveThread2 != null) receiveThread2.Abort();
        if (stream2 != null) stream2.Close();
        if (client2 != null) client2.Close();
    }
}

