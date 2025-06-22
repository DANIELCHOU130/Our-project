using UnityEngine;
using TMPro;
using System.Collections;

public class CardViewOnlyPanel : MonoBehaviour
{
    [Header("小面板 UI 元件")]
    public GameObject viewOnlyPanel;
    public TMP_Text viewOnlyCardNameText;
    public TMP_Text viewOnlyCardContentText;
    public TMP_Text viewOnlyCardMoneyText;
    public TMP_Text viewOnlyCardESGText;
    public TMP_Text viewOnlyCardKnowText;
    public TMP_Text viewOnlyPlayerNameText;

    [Header("設定")]
    public float autoCloseTime = 5f;

    private Coroutine closeCoroutine;

    void Start()
    {
        viewOnlyPanel.SetActive(false);

        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnReceiveCard += OnReceiveCardData;
        }
    }

    void OnDestroy()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.OnReceiveCard -= OnReceiveCardData;
        }
    }

    private void OnReceiveCardData(string cardData)
    {
        string[] parts = cardData.Split(',');

        if (parts.Length < 10)
        {
            Debug.LogWarning("收到卡片資料格式錯誤！");
            return;
        }

        viewOnlyPanel.SetActive(true);
        viewOnlyPlayerNameText.text = $"{parts[0]} 抽到一張事件卡！";
        viewOnlyCardNameText.text = $"卡片名稱: {parts[1]}";
        viewOnlyCardContentText.text = $"內容: {parts[2]}";
        viewOnlyCardMoneyText.text = $"金錢影響: {parts[3]}";
        viewOnlyCardESGText.text = $"ESG影響: {parts[4]}";
        viewOnlyCardKnowText.text = $"背景知識: {parts[5]}";

        if (closeCoroutine != null) StopCoroutine(closeCoroutine);
        closeCoroutine = StartCoroutine(AutoClosePanel());
    }

    private IEnumerator AutoClosePanel()
    {
        yield return new WaitForSeconds(autoCloseTime);
        viewOnlyPanel.SetActive(false);
    }
}
