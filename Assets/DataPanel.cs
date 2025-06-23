using System;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string connectionString =
        "Data Source=134.208.97.162\\SQL2022;Initial Catalog=ESGGAMEDB;User ID=LAB;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    public TMP_Text dataText;

    public void FetchGameData()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT gameid, gamerid, gamermoney, gameresg, boardid, typeid FROM dbo.nowgamedata WHERE gamerid = 6", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();

                        while (reader.Read())
                        {
                            int gameid = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            int gamerid = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            float money = reader.IsDBNull(2) ? 0f : Convert.ToSingle(reader["gamermoney"]);
                            string esg = reader.IsDBNull(3) ? "無資料" : reader.GetString(3);
                            int boardid = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                            int typeid = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

                            sb.AppendLine($"遊戲 ID: {gameid}, 玩家 ID: {gamerid}, 金錢: {money}, 結果: {esg}, 桌號: {boardid}, 類型 ID: {typeid}");
                        }

                        DisplayGameData(sb.ToString());
                    }
                    else
                    {
                        DisplayGameData("未找到 gamerid 6 的資料。");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"SQL 錯誤: {ex.Message}");
                DisplayGameData($"錯誤: {ex.Message}");
            }
        }
    }

    private void DisplayGameData(string gameData)
    {
        if (dataText != null)
            dataText.text = gameData;
        else
            Debug.LogError("未指定 TMP_Text 來顯示資料。");
    }
}
