using UnityEngine;
using System.Data.SqlClient;
using System.Collections;
using TMPro;

public class RoomMonitor : MonoBehaviour
{
    [Header("資料庫設定")]
    [SerializeField] private string connectionString = "Data Source=134.208.97.162\\SQL2022;Initial Catalog=ESGGAMEDB;User ID=LAB;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    [Header("玩家資訊")]
    public int myBoardId = 1;  // 自己所在的房間編號，啟動時必須設定

    [Header("監控條件")]
    public int maxRound = 20;          // 最多回合數
    public float targetESG = 100.0f;    // 目標 ESG 指數

    [Header("顯示用")]
    public TMP_Text roomDataText;      // 顯示玩家資料的 Text，可選填

    private int currentRound = 0;       // 當前回合數

    private void Start()
    {
        InvokeRepeating(nameof(MonitorRoomStatus), 5f, 5f); // 每 5 秒檢查一次
    }

    private void MonitorRoomStatus()
    {
        StartCoroutine(CheckRoomAndResetIfNeeded());
    }

    private IEnumerator CheckRoomAndResetIfNeeded()
    {
        yield return null; // 保留擴充空間，未來可以塞 loading 動畫等

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 查詢房間內所有玩家金錢和ESG
                string query = $"SELECT gamerid, gamermoney, gameresg FROM dbo.nowgamedata WHERE boardid = {myBoardId}";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                bool shouldReset = false;
                string collectedData = "";
                currentRound++; // 每次查詢算一次回合

                while (reader.Read())
                {
                    int gamerid = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    float gamermoney = reader.IsDBNull(1) ? 0.0f : reader.GetFloat(1);
                    float gameresg = reader.IsDBNull(2) ? 0.0f : float.Parse(reader.GetString(2));

                    collectedData += $"玩家ID: {gamerid}, 金錢: {gamermoney}, ESG: {gameresg}\n";

                    if (gamermoney <= 0 || gameresg >= targetESG)
                        shouldReset = true;
                }
                reader.Close();

                // 更新UI顯示（可選）
                if (roomDataText != null)
                {
                    roomDataText.text = collectedData;
                }

                // 判斷是否要重置
                if (shouldReset || currentRound >= maxRound)
                {
                    Debug.LogWarning("條件達成！即將歸零房間內所有玩家資料。");
                    ResetRoomPlayers();
                    currentRound = 0; // 重置回合數
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"資料庫錯誤：{ex.Message}");
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

                Debug.Log("房間玩家資料已成功重置！");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"重置資料失敗：{ex.Message}");
        }
    }
}
