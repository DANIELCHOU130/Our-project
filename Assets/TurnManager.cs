
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<string> playerOrder = new List<string>();
    private int currentTurnIndex = 0;
    private int currentRoundCount = 0;

    public string currentPlayer
    {
        get
        {
            if (playerOrder == null || playerOrder.Count == 0)
                return string.Empty;
            if (currentTurnIndex < 0 || currentTurnIndex >= playerOrder.Count)
                return string.Empty;
            return playerOrder[currentTurnIndex];
        }
    }

    public event Action<string> OnTurnChanged;

    private Coroutine autoRollCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnMessageReceived += HandleNetworkMessage;
        }
    }

    public void InitializeTurnOrder(List<string> sortedPlayerNames)
    {
        playerOrder = sortedPlayerNames;
        currentTurnIndex = 0;
        currentRoundCount = 1;

        Debug.Log($"回合初始化完成，第一位玩家是 {currentPlayer}");
        OnTurnChanged?.Invoke(currentPlayer);
        NotifyAllPlayersTurn();

        StartAutoRollTimer();
    }

    public void EndTurn()
    {
        if (playerOrder == null || playerOrder.Count == 0)
            return;

        currentTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;

        if (currentTurnIndex == 0)
        {
            currentRoundCount++;
            Debug.Log($"====== 進入第 {currentRoundCount} 回合 ======");
        }

        Debug.Log($"換人，現在是 {currentPlayer} 的回合");
        OnTurnChanged?.Invoke(currentPlayer);
        NotifyAllPlayersTurn();

        StartAutoRollTimer();
    }

    private void NotifyAllPlayersTurn()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.SendMessageToServer($"TURN,{currentPlayer}");
        }
    }

    private void HandleNetworkMessage(string msg)
    {
        if (msg.StartsWith("TURN,"))
        {
            string player = msg.Substring(5);
            currentTurnIndex = playerOrder.IndexOf(player);
            Debug.Log($"同步收到回合資訊，現在輪到 {player}");
            OnTurnChanged?.Invoke(currentPlayer);
            StartAutoRollTimer();
        }
    }

    public bool IsMyTurn()
    {
        return currentPlayer == NetworkClient.Instance?.playerName;
    }

    private void StartAutoRollTimer()
    {
        if (autoRollCoroutine != null)
            StopCoroutine(autoRollCoroutine);

        if (IsMyTurn())
            autoRollCoroutine = StartCoroutine(AutoRollDiceAfterDelay(40f));
    }

    private IEnumerator AutoRollDiceAfterDelay(float delaySeconds)
    {
        float elapsed = 0f;
        while (elapsed < delaySeconds)
        {
            if (!IsMyTurn()) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (IsMyTurn())
        {
            Debug.Log("⏰ 40秒未動作，自動擲骰！");
            FindFirstObjectByType<dicechange>()?.RollDiceAuto();

        }
    }

    public int GetCurrentRound()
    {
        return currentRoundCount;
    }
}
