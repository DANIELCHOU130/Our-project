using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject mapPanel;
    public GameObject playerStatusPanel;
    public GameObject otherPlayersPanel;

    [Header("Player Pieces")]
    public List<GameObject> playerPieces;

    [Header("Player Data UI")]
    public TMP_Text playerMoneyText;
    public TMP_Text playerESGText;

    [Header("Other Players Data UI")]
    public List<TMP_Text> otherPlayerNameTexts;
    public List<TMP_Text> otherPlayerMoneyTexts;
    public List<TMP_Text> otherPlayerESGTexts;

    [Header("Initial Values")]
    public Vector3 initialPiecePosition = new Vector3(-14f, 8.75f, 0f);
    public int initialMoney = 1000;
    public int initialESG = 0;

    private class PlayerData
    {
        public string name;
        public int money;
        public int esg;
    }

    private readonly List<PlayerData> allPlayers = new List<PlayerData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mapPanel.SetActive(false);
        playerStatusPanel.SetActive(false);
        otherPlayersPanel.SetActive(false);

        foreach (var piece in playerPieces)
            piece.SetActive(false);
    }

    public void InitGame()
    {
        mapPanel.SetActive(true);
        playerStatusPanel.SetActive(true);
        otherPlayersPanel.SetActive(true);

        allPlayers.Clear();

        for (int i = 0; i < playerPieces.Count; i++)
        {
            string playerName = i == 0 ? "我" : $"玩家{i + 1}";

            playerPieces[i].SetActive(true);
            playerPieces[i].transform.position = initialPiecePosition;
            playerPieces[i].name = playerName;

            allPlayers.Add(new PlayerData()
            {
                name = playerName,
                money = initialMoney,
                esg = initialESG
            });
        }

        RefreshPlayerStatus();
        RefreshOtherPlayersStatus();
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
            otherPlayerNameTexts[i - 1].text = allPlayers[i].name;
            otherPlayerMoneyTexts[i - 1].text = $"金錢：{allPlayers[i].money}";
            otherPlayerESGTexts[i - 1].text = $"ESG：{allPlayers[i].esg}";
        }
    }
}
