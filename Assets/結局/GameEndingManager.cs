using System;
using System.Data.SqlClient;
using UnityEngine;

public class GameEndingManager : MonoBehaviour
{
    [Header("結局資料")]
    public EndingData ending1;  // 均衡 玩家ESG < 0
    public EndingData ending2;  // E最大 > 5 玩家ESG < 0
    public EndingData ending3;  // S最大 > 5 玩家ESG < 0
    public EndingData ending4;  // G最大 > 5 玩家ESG < 0
    public EndingData ending5;  // 均衡,玩家ESG > 0
    public EndingData ending6;  // E最大 玩家ESG > 0
    public EndingData ending7;  // S最大 玩家ESG > 0
    public EndingData ending8;  // G最大 玩家ESG > 0
    public EndingData ending9;  // 勝利者 均衡 
    public EndingData ending10; // 勝利者 E最大
    public EndingData ending11; // 勝利者 S最大
    public EndingData ending12; // 勝利者 G最大

    public EndingScene endingScene;

    private string connectionString =
        "Data Source=134.208.97.162\\SQL2022;Initial Catalog=ESGGAMEDB;User ID=LAB;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    /// 
    /// 呼叫進入結局
    /// 
    public void CheckEnding(int gameId, int playerId, bool isWinner)
    {
        // 讀取 nowgamedata
        var playerData = GetPlayerData(gameId, playerId);

        // 判斷 token 是否全為 0
        bool noTokens = playerData.token1 == 0 && playerData.token2 == 0 &&
                        playerData.token3 == 0 && playerData.token4 == 0;

        if (!isWinner && noTokens)
        {
            PlayNormalEnding(playerData);
        }
        else if (isWinner && noTokens)
        {
            PlayWinnerEnding(playerData);
        }
    }

    private void PlayNormalEnding(PlayerInfo p)
    {
        if (p.esg < 0)
        {
            if (p.epoint == p.spoint && p.spoint == p.gpoint)
                PlayEnding(ending1); 
            else
                PlayEndingByHighest(p, ending2, ending3, ending4);
            return;
        }

        if (p.epoint == p.spoint && p.spoint == p.gpoint)
            PlayEnding(ending5);
        else
            PlayEndingByHighest(p, ending6, ending7, ending8);
    }

    private void PlayWinnerEnding(PlayerInfo p)
    {
        if (p.epoint == p.spoint && p.spoint == p.gpoint)
            PlayEnding(ending9);
        else
            PlayEndingByHighest(p, ending10, ending11, ending12);
    }

    private void PlayEndingByHighest(PlayerInfo p, EndingData eEnding, EndingData sEnding, EndingData gEnding)
    {
        int[] points = { p.epoint, p.spoint, p.gpoint };
        int maxIndex = Array.IndexOf(points, Mathf.Max(points));
        switch (maxIndex)
        {
            case 0: PlayEnding(eEnding); break;
            case 1: PlayEnding(sEnding); break;
            case 2: PlayEnding(gEnding); break;
        }
    }



    private void PlayEnding(EndingData ending)
    {
        if (ending != null)
        {
            endingScene.ShowEnding(ending);
            Debug.Log($"播放結局: {ending.endingTitle}");
        }
        else
        {
            Debug.LogWarning("未綁定對應的結局資料！");
        }
    }


    private PlayerInfo GetPlayerData(int gameId, int playerId)
    {
        PlayerInfo data = new PlayerInfo();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = @"
                SELECT epoint, spoint, gpoint, gameresg, token1, token2, token3, token4
                FROM nowgamedata
                WHERE gameid = @gameid AND gamerid = @playerid";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@gameid", gameId);
                cmd.Parameters.AddWithValue("@playerid", playerId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data.epoint = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        data.spoint = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        data.gpoint = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        data.esg = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        data.token1 = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                        data.token2 = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                        data.token3 = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                        data.token4 = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                    }
                }
            }
        }

        return data;
    }
}

[Serializable]
public class PlayerInfo
{
    public int epoint;
    public int spoint;
    public int gpoint;
    public int esg;
    public int token1;
    public int token2;
    public int token3;
    public int token4;
}
