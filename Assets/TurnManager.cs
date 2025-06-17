<<<<<<< HEAD
Ôªøusing System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
=======
using System;
using System.Collections.Generic;
using UnityEngine;
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34

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
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // ±“∞ Æ…µ˘•U±µ¶¨ API ∞TÆß°]TURN,xxx°^
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnReceiveMessage += HandleServerMessage;
        }
    }

    public void InitializeTurnOrder(List<string> sortedPlayerNames)
    {
        playerOrder = sortedPlayerNames;
        currentTurnIndex = 0;
        OnTurnChanged?.Invoke(currentPlayer);
<<<<<<< HEAD
        NotifyAllPlayersTurn();
        UpdateTurnUI();
        StartAutoRollCountdown();
=======

        NotifyTurnChange();
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    }

    public void EndTurn()
    {
        if (playerOrder == null || playerOrder.Count == 0) return;
<<<<<<< HEAD
=======

>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
        currentTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;
        OnTurnChanged?.Invoke(currentPlayer);
<<<<<<< HEAD
        NotifyAllPlayersTurn();
        UpdateTurnUI();
        StartAutoRollCountdown();
=======

        NotifyTurnChange();
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    }

    private void NotifyTurnChange()
    {
        if (NetworkClient.Instance != null)
        {
            string msg = $"TURN,{currentPlayer}";
            NetworkClient.Instance.SendMessageToServer(msg);
        }
    }

    private void UpdateTurnUI()
    {
        if (currentTurnText != null)
        {
            currentTurnText.text = $"ÁõÆÂâçÂõûÂêàÔºö{currentPlayer}";
        }
    }

    public bool IsMyTurn()
    {
        return currentPlayer == NetworkClient.Instance.myPlayerName;
    }

<<<<<<< HEAD
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
            Debug.Log("‚è± ÈÄæÊôÇÊú™Êì≤È™∞ÔºåËá™ÂãïÊì≤È™∞‰∏≠...");
            dicechange dice = FindObjectOfType<dicechange>();
            if (dice != null && !dice.IsRolling)
            {
                dice.RollDiceExternally();
=======
    private void HandleServerMessage(string message)
    {
        if (message.StartsWith("TURN,"))
        {
            string playerName = message.Substring(5);
            int index = playerOrder.IndexOf(playerName);

            if (index != -1)
            {
                currentTurnIndex = index;
                Debug.Log($"¶¯™Aæπ≥q™æ¥´§H°A≤{¶b¨O {playerName} ™∫¶^¶X");
                OnTurnChanged?.Invoke(playerName);
            }
            else
            {
                Debug.LogWarning($"¶¨®Ï•º™æ™±Æa¶W∫Ÿ™∫ TURN ∞TÆß°G{playerName}");
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
            }
        }
    }
}
