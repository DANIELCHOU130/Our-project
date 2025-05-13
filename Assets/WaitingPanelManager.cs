using System.Collections;
using UnityEngine;
using TMPro;

public class WaitingPanelManager : MonoBehaviour
{
    public GameServerClient client;
    public TMP_Text statusText;
    public GameObject waitingPanel;

    private int currentGameId;
    private Coroutine checkCoroutine;

    public void StartWaiting(int gameId)
    {
        currentGameId = gameId;
        waitingPanel.SetActive(true);
        statusText.text = "���ݨ�L���a�[�J��...";
        checkCoroutine = StartCoroutine(CheckLoop());
    }

    IEnumerator CheckLoop()
    {
        while (true)
        {
            client.SendCommand($"CHECK,{currentGameId}");
            yield return new WaitForSeconds(2f);
        }
    }

    public void HandleServerResponse(string msg)
    {
        if (msg.StartsWith("WAITING"))
        {
            string[] parts = msg.Split(',');
            if (parts.Length >= 2)
                statusText.text = $"�ثe�H��: {parts[1]}/4�A���ݶ}�l...";
        }
        else if (msg == "START_GAME")
        {
            statusText.text = "�H�Ƥw���A�i�J�C��...";
            StopCoroutine(checkCoroutine);
            Invoke("LoadGameScene", 2f); // �����������
        }
    }

    void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
