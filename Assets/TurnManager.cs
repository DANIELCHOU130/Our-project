using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<string> playerOrder = new List<string>(); // 排好順序的玩家 ID
    private int currentTurnIndex = 0;
    public string currentPlayer => playerOrder[currentTurnIndex];

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
        return currentPlayer == NetworkClient.Instance.myPlayerName;
    }
}

