using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<string> playerOrder = new List<string>(); // �Ʀn���Ǫ����a ID
    private int currentTurnIndex = 0;

    public string currentPlayer
    {
        get
        {
            if (playerOrder == null || playerOrder.Count == 0)
            {
                Debug.LogWarning("�iTurnManager�jplayerOrder �|����l�ơI");
                return string.Empty;
            }

            if (currentTurnIndex < 0 || currentTurnIndex >= playerOrder.Count)
            {
                Debug.LogWarning($"�iTurnManager�jcurrentTurnIndex {currentTurnIndex} �W�X�d��I");
                return string.Empty;
            }

            return playerOrder[currentTurnIndex];
        }
    }

    public event Action<string> OnTurnChanged; // ���~���q�\�^�X�ܧ�ƥ�

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeTurnOrder(List<string> sortedPlayerNames)
    {
        playerOrder = sortedPlayerNames;
        currentTurnIndex = 0;

        Debug.Log($"�^�X��l�Ƨ����A�Ĥ@�쪱�a�O {currentPlayer}");
        OnTurnChanged?.Invoke(currentPlayer);

        NotifyAllPlayersTurn();
    }

    public void EndTurn()
    {
        if (playerOrder == null || playerOrder.Count == 0)
        {
            Debug.LogWarning("�iTurnManager�jEndTurn �I�s�� playerOrder �|����l�ơI");
            return;
        }

        currentTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;

        Debug.Log($"���H�A�{�b�O {currentPlayer} ���^�X");
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
