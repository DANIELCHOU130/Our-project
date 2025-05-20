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

    void Start()
    {
        waitingPanel.SetActive(false); 
    }

    public void StartWaiting(int gameId)
    {
        currentGameId = gameId;
        waitingPanel.SetActive(true);
        statusText.text = "单ㄤ產い...";
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
                statusText.text = $"ヘ玡计: {parts[1]}/4单秨﹍...";
        }
        else if (msg == "START_GAME")
        {
            statusText.text = "计骸秈笴栏...";
            StopCoroutine(checkCoroutine);
            Invoke("LoadGameScene", 2f); // ┑筐ち传初春
        }
    }

    void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
