using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<string> playerOrder = new List<string>(); // 排好順序的玩家 ID
    private int currentTurnIndex = 0;

    public string currentPlayer
    {
        get
        {
            if (playerOrder == null || playerOrder.Count == 0) return string.Empty;
            if (currentTurnIndex < 0 || currentTurnIndex >= playerOrder.Count) return string.Empty;
            return playerOrder[currentTurnIndex];
        }
    }

    public event Action<string> OnTurnChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 啟動時註冊接收 API 訊息（TURN,xxx）
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnReceiveMessage += HandleServerMessage;
        }
    }

    public void InitializeTurnOrder(List<string> sortedPlayerNames)
    {
        playerOrder = sortedPlayerNames;
        currentTurnIndex = 0;

        Debug.Log($"回合初始化完成，第一位玩家是 {currentPlayer}");
        OnTurnChanged?.Invoke(currentPlayer);

        NotifyTurnChange();
    }

    public void EndTurn()
    {
        if (playerOrder == null || playerOrder.Count == 0) return;

        currentTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;

        Debug.Log($"換人，現在是 {currentPlayer} 的回合");
        OnTurnChanged?.Invoke(currentPlayer);

        NotifyTurnChange();
    }

    private void NotifyTurnChange()
    {
        if (NetworkClient.Instance != null)
        {
            string msg = $"TURN,{currentPlayer}";
            NetworkClient.Instance.SendMessageToServer(msg);
        }
    }

    public bool IsMyTurn()
    {
        if (string.IsNullOrEmpty(currentPlayer)) return false;

        return currentPlayer == NetworkClient.Instance.myPlayerName;
    }

    private void HandleServerMessage(string message)
    {
        if (message.StartsWith("TURN,"))
        {
            string playerName = message.Substring(5);
            int index = playerOrder.IndexOf(playerName);

            if (index != -1)
            {
                currentTurnIndex = index;
                Debug.Log($"伺服器通知換人，現在是 {playerName} 的回合");
                OnTurnChanged?.Invoke(playerName);
            }
            else
            {
                Debug.LogWarning($"收到未知玩家名稱的 TURN 訊息：{playerName}");
            }
        }
    }
}
