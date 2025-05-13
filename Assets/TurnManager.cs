using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<string> playerOrder = new List<string>(); // 排好順序的玩家 ID
    private int currentTurnIndex = 0;

    public string currentPlayer
    {
        get
        {
            if (playerOrder == null || playerOrder.Count == 0)
            {
                Debug.LogWarning("【TurnManager】playerOrder 尚未初始化！");
                return string.Empty;
            }

            if (currentTurnIndex < 0 || currentTurnIndex >= playerOrder.Count)
            {
                Debug.LogWarning($"【TurnManager】currentTurnIndex {currentTurnIndex} 超出範圍！");
                return string.Empty;
            }

            return playerOrder[currentTurnIndex];
        }
    }

    public event Action<string> OnTurnChanged; // 讓外部訂閱回合變更事件

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeTurnOrder(List<string> sortedPlayerNames)
    {
        playerOrder = sortedPlayerNames;
        currentTurnIndex = 0;

        Debug.Log($"回合初始化完成，第一位玩家是 {currentPlayer}");
        OnTurnChanged?.Invoke(currentPlayer);

        NotifyAllPlayersTurn();
    }

    public void EndTurn()
    {
        if (playerOrder == null || playerOrder.Count == 0)
        {
            Debug.LogWarning("【TurnManager】EndTurn 呼叫時 playerOrder 尚未初始化！");
            return;
        }

        currentTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;

        Debug.Log($"換人，現在是 {currentPlayer} 的回合");
        OnTurnChanged?.Invoke(currentPlayer);

        NotifyAllPlayersTurn();
    }

    private void NotifyAllPlayersTurn()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.SendMessageToServer($"TURN,{currentPlayer}");
        }
    }

    public bool IsMyTurn()
    {
        if (string.IsNullOrEmpty(currentPlayer)) return false;

        return currentPlayer == NetworkClient.Instance.myPlayerName;
    }
}
