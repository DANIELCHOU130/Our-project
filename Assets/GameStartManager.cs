using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mapPanel;
    public GameObject playerStatusPanel;
    public GameObject otherPlayersPanel;

    [Header("Player Pieces")]
    public List<GameObject> playerPieces;

    [Header("Player Data UI")]
    public TMP_Text playerMoneyText;
    public TMP_Text playerESGText;
    public TMP_Text currentPlayerTurnText;

    [Header("Other Players Data UI")]
    public List<TMP_Text> otherPlayerNameTexts;
    public List<TMP_Text> otherPlayerMoneyTexts;
    public List<TMP_Text> otherPlayerESGTexts;

    [Header("Initial Values")]
    public Vector3 initialPiecePosition = new Vector3(-14f, 8.75f, 0f);
    public int initialMoney = 1000;
    public int initialESG = 0;

    public static GameManager Instance;

    private int roundCount = 0;
    private const int SettleRoundInterval = 4;

    private class PlayerData
    {
        public string name;
        public int money;
        public int esg;
        public List<string> chips = new List<string>();
        public bool isAcquired => chips.Count == 0;
    }

    private List<PlayerData> allPlayers = new List<PlayerData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mapPanel.SetActive(false);
        foreach (var piece in playerPieces)
            piece.SetActive(false);

        playerStatusPanel.SetActive(false);
        otherPlayersPanel.SetActive(false);
    }

    public void InitGame()
    {
        mapPanel.SetActive(true);
        playerStatusPanel.SetActive(true);
        otherPlayersPanel.SetActive(true);

        foreach (var piece in playerPieces)
        {
            piece.SetActive(true);
            piece.transform.position = initialPiecePosition;
        }

        allPlayers.Clear();
        allPlayers.Add(new PlayerData { name = "我", money = initialMoney, esg = initialESG });
        allPlayers.Add(new PlayerData { name = "玩家2", money = initialMoney, esg = initialESG });
        allPlayers.Add(new PlayerData { name = "玩家3", money = initialMoney, esg = initialESG });
        allPlayers.Add(new PlayerData { name = "玩家4", money = initialMoney, esg = initialESG });

        foreach (var p in allPlayers)
        {
            p.chips = new List<string> { "ENERGREEN", "R-Tech", "CCTBank", "LightNet", "R-Tech" };
        }

        RefreshPlayerStatus();
        RefreshOtherPlayersStatus();
    }

    public void NextRound()
    {
        roundCount++;
        Debug.Log($"目前為第 {roundCount} 回合");

        if (roundCount % SettleRoundInterval == 0)
        {
            Debug.Log("進行籌碼結算");
            SettleChips();
        }
    }

    public void SettleChips()
    {
        if (allPlayers.Any(p => p.isAcquired)) return;

        List<int> scores = allPlayers.Select(GetPlayerScore).ToList();

        int minIndex = scores.IndexOf(scores.Min());
        int maxIndex = scores.IndexOf(scores.Max());

        if (minIndex == maxIndex) return;

        PlayerData loser = allPlayers[minIndex];
        PlayerData winner = allPlayers[maxIndex];

        if (loser.chips.Count == 0) return;

        // 讓勝者選擇要哪個籌碼（目前先選第一個，有 UI 可自訂）
        string chosenChip = loser.chips[0];
        loser.chips.Remove(chosenChip);
        winner.chips.Add(chosenChip);

        Debug.Log($"{winner.name} 從 {loser.name} 拿走籌碼：{chosenChip}");

        if (loser.chips.Count == 0)
        {
            HandleAcquisition(loser);
        }

        RefreshPlayerStatus();
        RefreshOtherPlayersStatus();
    }

    private int GetPlayerScore(PlayerData player)
    {
        int bonus = GetScoreMultiplier(player.money);
        return player.money + player.esg * bonus;
    }

    private int GetScoreMultiplier(int money)
    {
        if (money >= 10000) return 5;
        if (money >= 5000) return 4;
        if (money >= 2000) return 3;
        if (money >= 1000) return 2;
        return 1;
    }

    private void HandleAcquisition(PlayerData loser)
    {
        int totalMoney = loser.money;
        int totalChips = allPlayers.Where(p => !p.isAcquired).Sum(p => p.chips.Count);

        foreach (var p in allPlayers)
        {
            if (p == loser || p.isAcquired) continue;

            float ratio = (float)p.chips.Count / totalChips;
            int gain = Mathf.RoundToInt(totalMoney * ratio);
            p.money += gain;
            Debug.Log($"{p.name} 從併購中分得 {gain} 元");
        }

        Debug.Log($"{loser.name} 被併購，退出遊戲");

        // 你可在這裡加上遊戲結束的條件判斷與面板觸發
    }

    private void RefreshPlayerStatus()
    {
        var myData = allPlayers[0];
        playerMoneyText.text = $"金錢：{myData.money}";
        playerESGText.text = $"ESG：{myData.esg}";
    }

    private void RefreshOtherPlayersStatus()
    {
        for (int i = 1; i < allPlayers.Count; i++)
        {
            var p = allPlayers[i];
            otherPlayerNameTexts[i - 1].text = p.name;
            otherPlayerMoneyTexts[i - 1].text = $"金錢：{p.money}";
            otherPlayerESGTexts[i - 1].text = $"ESG：{p.esg}";
        }
    }

    public void AddMoneyToCurrentPlayer(int amount)
    {
        allPlayers[0].money += amount;
        RefreshPlayerStatus();
    }

    public void AddESGToCurrentPlayer(int amount)
    {
        allPlayers[0].esg += amount;
        RefreshPlayerStatus();
    }
}
