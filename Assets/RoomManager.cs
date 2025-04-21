using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    private string connectionString = "Data Source=192.168.1.175\\SQLEXPRESS03;Initial Catalog=ESGGAMEDB;User ID=Dan;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    public TMP_InputField roomIdInputField; // 房間 ID 輸入框
    public TMP_Dropdown playerDropdown; // 玩家選單
    public TMP_Text playerDataText; // 顯示玩家資訊
    public Button resetButton;
    public Button roomGetButton;

    private List<int> playerIds = new List<int>();

    void Start()
    {
        roomGetButton.onClick.AddListener(FetchRoomPlayers);
        resetButton.onClick.AddListener(SetPlayersDataToZero);

        playerDropdown.ClearOptions();
        playerDataText.text = "請選擇玩家以顯示數據。";

        playerDropdown.onValueChanged.AddListener(delegate { DisplayPlayerData(); });
    }

    // 查詢房間內玩家
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
                string query = "SELECT gamerid FROM dbo.nowgamedata WHERE boardid = @roomId";
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

                if (options.Count == 0)
                {
                    options.Add("無玩家數據");
                    playerDataText.text = "查詢結果：此房間無玩家數據記錄";
                }

                playerDropdown.AddOptions(options);
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL 錯誤 - {ex.Message}\n{ex.StackTrace}");
        }
    }

    // 顯示選中玩家的資訊
    public void DisplayPlayerData()
    {
        int selectedIndex = playerDropdown.value;

        if (playerIds.Count == 0 || selectedIndex < 0 || selectedIndex >= playerIds.Count)
        {
            playerDataText.text = "請選擇有效的玩家來顯示數據。";
            return;
        }

        int playerId = playerIds[selectedIndex];

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT gamermoney, gameresg FROM dbo.nowgamedata WHERE gamerid = @playerId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@playerId", playerId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    float money = reader.IsDBNull(0) ? 0 : Convert.ToSingle(reader.GetValue(0));
                    string esg = reader.IsDBNull(1) ? "無" : reader.GetString(1);
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
        catch (InvalidCastException ex)
        {
            Debug.LogError($"轉型失敗: {ex.Message}");
        }
    }

    // 將該房間的玩家資料重設為 0
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
                string query = "UPDATE dbo.nowgamedata SET gamermoney = 0, gameresg = '0' WHERE boardid = @roomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                int rowsAffected = cmd.ExecuteNonQuery();
                Debug.Log($"已將 {rowsAffected} 位玩家數據重置為 0。");

                playerDataText.text = "所有玩家數據已重置！";
                FetchRoomPlayers();
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL 錯誤: {ex.Message}");
        }
    }
}
