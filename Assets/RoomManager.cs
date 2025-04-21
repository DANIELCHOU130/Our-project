using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    private string connectionString = "Data Source=192.168.1.175\\\\SQLEXPRESS03;Initial Catalog=ESGGAMEDB;User ID=Dan;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    public TMP_InputField roomIdInputField; // �ж� ID ����J��
    public TMP_Dropdown playerDropdown; // ��ܪ��a���U�Կ��
    public TMP_Text playerDataText; // ��ܪ��a�ƾڪ��奻
    public Button resetButton;

    private List<int> playerIds = new List<int>();

    void Start()
    {
        resetButton.onClick.AddListener(SetPlayersDataToZero);
    }

    // �d�߬Y�өж������a
    public void FetchRoomPlayers()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("�п�J�ж� ID�I");
            return;
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = $"SELECT gamerid FROM dbo.nowgamedata WHERE boardid = @roomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                SqlDataReader reader = cmd.ExecuteReader();
                playerDropdown.ClearOptions();
                playerIds.Clear();

                List<string> options = new List<string>();
                while (reader.Read())
                {
                    int playerId = reader.GetInt32(0);
                    playerIds.Add(playerId);
                    options.Add($"���a {playerId}");
                }

                playerDropdown.AddOptions(options);
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL ���~: {ex.Message}");
        }
    }

    // ��ܿ襤���a���ƾ�
    public void DisplayPlayerData()
    {
        int selectedIndex = playerDropdown.value;
        if (selectedIndex < 0 || selectedIndex >= playerIds.Count)
        {
            Debug.LogError("�п�ܦ��Ī����a�I");
            return;
        }

        int playerId = playerIds[selectedIndex];

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = $"SELECT gamermoney, gameresg FROM dbo.nowgamedata WHERE gamerid = @playerId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@playerId", playerId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    float money = reader.GetFloat(0);
                    string esg = reader.GetString(1);
                    playerDataText.text = $"����: {money}\nESG: {esg}";
                }
                else
                {
                    playerDataText.text = "�����Ӫ��a���ƾڡC";
                }
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL ���~: {ex.Message}");
        }
    }

    public void SetPlayersDataToZero()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("�п�J�ж� ID�I");
            return;
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = $"UPDATE dbo.nowgamedata SET gamermoney = 0, gameresg = '0' WHERE boardid = @roomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                int rowsAffected = cmd.ExecuteNonQuery();
                Debug.Log($"�w�N {rowsAffected} �쪱�a�ƾڭ��m�� 0�C");
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL ���~: {ex.Message}");
        }
    }
}
