using System;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // 資料庫連接字串
    private string connectionString = "Data Source=192.168.1.175\\SQLEXPRESS03;Initial Catalog=ESGGAMEDB;User ID=Dan;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    // 用於顯示資料的 TMP_Text 組件
    public TMP_Text dataText;

    // 觸發資料加載的按鈕事件
    public void FetchGameData()
    {
        try
        {
            // 建立 SQL 連接物件
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // 打開資料庫連接
                conn.Open();
                Debug.Log("SQL 連線成功！");

                // SQL 查詢語句
                string query = "SELECT gameid, gamerid, gamermoney, gameresg, boardid, typeid FROM dbo.nowgamedata WHERE gamerid = 6";
                SqlCommand cmd = new SqlCommand(query, conn);

                // 執行查詢並獲取資料
                SqlDataReader reader = cmd.ExecuteReader();

                // 如果查詢有返回資料
                if (reader.HasRows)
                {
                    string gameData = "";  // 用於保存顯示的遊戲資料

                    // 逐行讀取資料
                    while (reader.Read())
                    {
                        // **修正資料類型轉換**
                        int gameid = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader["gameid"]);
                        int gamerid = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader["gamerid"]);
                        float gamermoney = reader.IsDBNull(2) ? 0.0f : Convert.ToSingle(reader["gamermoney"]);
                        string gameresg = reader.IsDBNull(3) ? "無資料" : reader.GetString(3);
                        int boardid = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader["boardid"]);
                        int typeid = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader["typeid"]);

                        // 將每行資料格式化為字串並累加
                        gameData += $"遊戲 ID: {gameid}, 玩家 ID: {gamerid}, 金錢: {gamermoney}, 結果: {gameresg}, 桌號: {boardid}, 類型 ID: {typeid}\n";
                    }

                    // 更新 UI 顯示獲取到的資料
                    DisplayGameData(gameData);
                }
                else
                {
                    // 沒有找到資料時的提示
                    Debug.Log("沒有找到符合條件的資料！");
                    DisplayGameData("未找到 gamerid 6 的資料。");
                }
            }
        }
        catch (Exception ex)
        {
            // 捕獲並顯示異常錯誤
            Debug.LogError($"SQL 錯誤: {ex.Message}");
            DisplayGameData($"錯誤: {ex.Message}");
        }
    }

    // 顯示遊戲資料到 UI 的方法
    private void DisplayGameData(string gameData)
    {
        if (dataText != null)
        {
            dataText.text = gameData;
        }
        else
        {
            Debug.LogError("未指定 TMP_Text 來顯示資料。");
        }
    }
}
