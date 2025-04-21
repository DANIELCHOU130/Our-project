using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    private string connectionString = "Data Source=192.168.1.175\\\\SQLEXPRESS03;Initial Catalog=ESGGAMEDB;User ID=Dan;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    public TMP_InputField roomIdInputField; // 房間 ID 的輸入框
    public TMP_Dropdown playerDropdown; // 顯示玩家的下拉選單
    public TMP_Text playerDataText; // 顯示玩家數據的文本
    public Button resetButton;

    private List<int> playerIds = new List<int>();

    void Start()
    {
        resetButton.onClick.AddListener(SetPlayersDataToZero);
    }

    // 查詢某個房間的玩家
    public void FetchRoomPlayers()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("請輸入房間 ID！");
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
                    options.Add($"玩家 {playerId}");
                }

                playerDropdown.AddOptions(options);
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL 錯誤: {ex.Message}");
        }
    }

    // 顯示選中玩家的數據
    public void DisplayPlayerData()
    {
        int selectedIndex = playerDropdown.value;
        if (selectedIndex < 0 || selectedIndex >= playerIds.Count)
        {
            Debug.LogError("請選擇有效的玩家！");
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
                    playerDataText.text = $"金錢: {money}\nESG: {esg}";
                }
                else
                {
                    playerDataText.text = "未找到該玩家的數據。";
                }
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL 錯誤: {ex.Message}");
        }
    }

    public void SetPlayersDataToZero()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("請輸入房間 ID！");
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
                Debug.Log($"已將 {rowsAffected} 位玩家數據重置為 0。");
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL 錯誤: {ex.Message}");
        }
    }
}
