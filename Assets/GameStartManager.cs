using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mapPanel;
    public GameObject playerStatusPanel;
    public GameObject otherPlayersPanel;

    [Header("Player Pieces")]
    public List<GameObject> playerPieces; // ��4�ӴѤl�Զi��

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

    private List<PlayerData> allPlayers = new List<PlayerData>();

    void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        // 1. ��ܭ��O
        mapPanel.SetActive(true);
        playerStatusPanel.SetActive(true);
        otherPlayersPanel.SetActive(true);

        // 2. ��l�ƴѤl��m
        foreach (var piece in playerPieces)
        {
            piece.transform.position = initialPiecePosition;
        }

        // 3. ��l�ƪ��a�ƾ�
        allPlayers.Clear();
        allPlayers.Add(new PlayerData() { name = "��", money = initialMoney, esg = initialESG });
        allPlayers.Add(new PlayerData() { name = "���a2", money = initialMoney, esg = initialESG });
        allPlayers.Add(new PlayerData() { name = "���a3", money = initialMoney, esg = initialESG });
        allPlayers.Add(new PlayerData() { name = "���a4", money = initialMoney, esg = initialESG });

        // 4. ��sUI���
        RefreshPlayerStatus();
        RefreshOtherPlayersStatus();
    }

    private void RefreshPlayerStatus()
    {
        var myData = allPlayers[0];
        playerMoneyText.text = "�����G" + myData.money.ToString();
        playerESGText.text = "ESG�G" + myData.esg.ToString();
    }

    private void RefreshOtherPlayersStatus()
    {
        for (int i = 1; i < allPlayers.Count; i++)
        {
            otherPlayerNameTexts[i - 1].text = allPlayers[i].name;
            otherPlayerMoneyTexts[i - 1].text = "�����G" + allPlayers[i].money.ToString();
            otherPlayerESGTexts[i - 1].text = "ESG�G" + allPlayers[i].esg.ToString();
        }
    }
}
