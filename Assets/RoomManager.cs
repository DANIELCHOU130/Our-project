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
    public Button roomGetButton;

    private List<int> playerIds = new List<int>();

    void Start()
    {
        roomGetButton.onClick.AddListener(FetchRoomPlayers);
        resetButton.onClick.AddListener(SetPlayersDataToZero);

        playerDropdown.ClearOptions();
        playerDataText.text = "請選擇玩家以顯示數據。";

        // 設定下拉選單的事件監聽器
        playerDropdown.onValueChanged.AddListener(delegate { DisplayPlayerData(); });
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
                string query = "SELECT gamerid FROM dbo.nowgamedata WHERE boardid = @roomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                SqlDataReader reader = cmd.ExecuteReader();

                playerDropdown.ClearOptions(); // 每次刷新先清空選項
                playerIds.Clear();

                List<string> options = new List<string>();
                while (reader.Read())
                {
                    int playerId = reader.GetInt32(0); // 取得玩家 ID
                    playerIds.Add(playerId);
                    options.Add($"玩家 {playerId}");
                }

                if (options.Count == 0)
                {
                    // 如果無玩家數據
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

    // 顯示選中玩家的數據
    public void DisplayPlayerData()
    {
        int selectedIndex = playerDropdown.value;

        // 如果選項無效或無玩家數據，則跳出
        if (playerIds.Count == 0 || selectedIndex <= 0)
        {
            playerDataText.text = "請選擇有效的玩家來顯示數據。";
            return;
        }

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
                string query = "SELECT gamermoney, gameresg FROM dbo.nowgamedata WHERE gamerid = @playerId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@playerId", playerId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    float money = reader.IsDBNull(0) ? 0 : reader.GetFloat(0); // 從數據庫中讀取金錢數據（避免空值錯誤）
                    string esg = reader.IsDBNull(1) ? "無" : reader.GetString(1); // 處理可能為 NULL 的 ESG 數據
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

    // 重置玩家數據功能
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

                // 清空數據顯示框
                playerDataText.text = "所有玩家數據已重置！";

                // 更新下拉框內容
                FetchRoomPlayers();
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL 錯誤: {ex.Message}");
        }
    }
}
