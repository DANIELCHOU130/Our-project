using UnityEngine;
using System.Data.SqlClient;
using System.Collections;
using TMPro;

public class RoomMonitor : MonoBehaviour
{
    [Header("��Ʈw�]�w")]
    [SerializeField] private string connectionString = "Data Source=134.208.97.162\\SQL2022;Initial Catalog=ESGGAMEDB;User ID=LAB;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    [Header("���a��T")]
    public int myBoardId = 1;  // �ۤv�Ҧb���ж��s���A�Ұʮɥ����]�w

    [Header("�ʱ�����")]
    public int maxRound = 20;          // �̦h�^�X��
    public float targetESG = 100.0f;    // �ؼ� ESG ����

    [Header("��ܥ�")]
    public TMP_Text roomDataText;      // ��ܪ��a��ƪ� Text�A�i���

    private int currentRound = 0;       // ��e�^�X��

    private void Start()
    {
        InvokeRepeating(nameof(MonitorRoomStatus), 5f, 5f); // �C 5 ���ˬd�@��
    }

    private void MonitorRoomStatus()
    {
        StartCoroutine(CheckRoomAndResetIfNeeded());
    }

    private IEnumerator CheckRoomAndResetIfNeeded()
    {
        yield return null; // �O�d�X�R�Ŷ��A���ӥi�H�� loading �ʵe��

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // �d�ߩж����Ҧ����a�����MESG
                string query = $"SELECT gamerid, gamermoney, gameresg FROM dbo.nowgamedata WHERE boardid = {myBoardId}";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                bool shouldReset = false;
                string collectedData = "";
                currentRound++; // �C���d�ߺ�@���^�X

                while (reader.Read())
                {
                    int gamerid = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    float gamermoney = reader.IsDBNull(1) ? 0.0f : reader.GetFloat(1);
                    float gameresg = reader.IsDBNull(2) ? 0.0f : float.Parse(reader.GetString(2));

                    collectedData += $"���aID: {gamerid}, ����: {gamermoney}, ESG: {gameresg}\n";

                    if (gamermoney <= 0 || gameresg >= targetESG)
                        shouldReset = true;
                }
                reader.Close();

                // ��sUI��ܡ]�i��^
                if (roomDataText != null)
                {
                    roomDataText.text = collectedData;
                }

                // �P�_�O�_�n���m
                if (shouldReset || currentRound >= maxRound)
                {
                    Debug.LogWarning("����F���I�Y�N�k�s�ж����Ҧ����a��ơC");
                    ResetRoomPlayers();
                    currentRound = 0; // ���m�^�X��
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"��Ʈw���~�G{ex.Message}");
        }
    }

    private void ResetRoomPlayers()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string resetQuery = $"UPDATE dbo.nowgamedata SET gamermoney = 0, gameresg = '0' WHERE boardid = {myBoardId}";
                SqlCommand cmd = new SqlCommand(resetQuery, conn);
                cmd.ExecuteNonQuery();

                Debug.Log("�ж����a��Ƥw���\���m�I");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���m��ƥ��ѡG{ex.Message}");
        }
    }
}
