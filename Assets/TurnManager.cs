using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<string> playerOrder = new List<string>();
    private int currentTurnIndex = 0;

    public TMP_Text currentTurnText;
    public float autoRollDelay = 40f;
    private Coroutine autoRollCoroutine;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeTurnOrder(List<string> sortedPlayerNames)
    {
        playerOrder = sortedPlayerNames;
        currentTurnIndex = 0;
        OnTurnChanged?.Invoke(currentPlayer);
        NotifyAllPlayersTurn();
        UpdateTurnUI();
        StartAutoRollCountdown();
    }

    public void EndTurn()
    {
        if (playerOrder == null || playerOrder.Count == 0) return;
        currentTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;
        OnTurnChanged?.Invoke(currentPlayer);
        NotifyAllPlayersTurn();
        UpdateTurnUI();
        StartAutoRollCountdown();
    }

    private void NotifyAllPlayersTurn()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.SendMessageToServer($"TURN,{currentPlayer}");
        }
    }

    private void UpdateTurnUI()
    {
        if (currentTurnText != null)
        {
            currentTurnText.text = $"目前回合：{currentPlayer}";
        }
    }

    public bool IsMyTurn()
    {
        return currentPlayer == NetworkClient.Instance.myPlayerName;
    }

    private void StartAutoRollCountdown()
    {
        if (autoRollCoroutine != null)
            StopCoroutine(autoRollCoroutine);

        if (IsMyTurn())
        {
            autoRollCoroutine = StartCoroutine(AutoRollAfterDelay());
        }
    }

    private IEnumerator AutoRollAfterDelay()
    {
        yield return new WaitForSeconds(autoRollDelay);

        if (IsMyTurn())
        {
            Debug.Log("⏱ 逾時未擲骰，自動擲骰中...");
            dicechange dice = FindObjectOfType<dicechange>();
            if (dice != null && !dice.IsRolling)
            {
                dice.RollDiceExternally();
            }
        }
    }
}
