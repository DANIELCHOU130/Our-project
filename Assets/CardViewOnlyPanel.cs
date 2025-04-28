using UnityEngine;
using TMPro;
using System.Collections;

public class CardViewOnlyPanel : MonoBehaviour
{
    public GameObject viewOnlyPanel;           // 🔥 小面板本體
    public TMP_Text viewOnlyCardNameText;       // 卡片名稱
    public TMP_Text viewOnlyCardContentText;    // 卡片內容
    public TMP_Text viewOnlyCardMoneyText;      // 金錢變化
    public TMP_Text viewOnlyCardESGText;        // ESG變化
    public TMP_Text viewOnlyCardKnowText;       // 背景知識
    public TMP_Text viewOnlyPlayerNameText;     // 誰抽到這張卡

    public float autoCloseTime = 5f;             // 🔥 自動關閉秒數（預設5秒）

    private Coroutine closeCoroutine;           // 🔥 記錄關閉用的協程

    void Start()
    {
        viewOnlyPanel.SetActive(false); // 預設是關閉的

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
        // 收到的格式：PlayerX,cardname,cardin,cardmoney,cardesg,cardknow,cardtype,choose1,choose2,choose3,choose4
        string[] parts = cardData.Split(',');

        if (parts.Length < 10)
        {
            Debug.LogWarning("收到卡片資料格式錯誤！");
            return;
        }

        string playerName = parts[0];
        string cardName = parts[1];
        string cardContent = parts[2];
        string cardMoney = parts[3];
        string cardESG = parts[4];
        string cardKnow = parts[5];
        string cardType = parts[6];

        // 顯示內容
        viewOnlyPanel.SetActive(true);

        viewOnlyPlayerNameText.text = $"{playerName} 抽到一張事件卡！";
        viewOnlyCardNameText.text = $"卡片名稱: {cardName}";
        viewOnlyCardContentText.text = $"內容: {cardContent}";
        viewOnlyCardMoneyText.text = $"金錢影響: {cardMoney}";
        viewOnlyCardESGText.text = $"ESG影響: {cardESG}";
        viewOnlyCardKnowText.text = $"背景知識: {cardKnow}";

        // 🔥 自動關閉
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }
        closeCoroutine = StartCoroutine(AutoClosePanel());
    }

    private IEnumerator AutoClosePanel()
    {
        yield return new WaitForSeconds(autoCloseTime);
        viewOnlyPanel.SetActive(false);
    }
}
