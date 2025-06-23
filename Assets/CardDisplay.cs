using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class CardDisplay : MonoBehaviour
{
    public GameObject panelS, panelE, panelM, panelG, panelC;
    public GameObject viewOnlyPanel;
    public TMP_Text viewOnlyCardNameText, viewOnlyCardContentText, viewOnlyCardMoneyText, viewOnlyCardESGText, viewOnlyCardKnowText, viewOnlyPlayerNameText;

    public TextMeshProUGUI cardNameTextS, cardContentTextS, cardMoneyTextS, cardESGTextS, cardKnowTextS, cardTypeTextS;
    public TextMeshProUGUI cardNameTextE, cardContentTextE, cardMoneyTextE, cardESGTextE, cardKnowTextE, cardTypeTextE;
    public TextMeshProUGUI cardNameTextM;
    public TextMeshProUGUI cardNameTextG, cardContentTextG, cardMoneyTextG, cardESGTextG, cardKnowTextG, cardTypeTextG;

    public Button chooseButton1, chooseButton2, chooseButton3, chooseButton4;
    public TextMeshProUGUI chooseText1, chooseText2, chooseText3, chooseText4;

    public TMP_Text textDisplay;
    public Button closeButtonS, closeButtonE, closeButtonM, closeButtonG;

    public GameObject errorPanel;
    public TMP_Text errorText;
    public Button closeErrorButton;

    private CardData currentCard;
    private float modifiedMoney;
    private float modifiedESG;

    void Start()
    {
        panelS.SetActive(false);
        panelE.SetActive(false);
        panelM.SetActive(false);
        panelG.SetActive(false);
        panelC.SetActive(false);
        viewOnlyPanel.SetActive(false);
        errorPanel.SetActive(false);

        closeButtonS.onClick.AddListener(() => ClosePanel(panelS));
        closeButtonE.onClick.AddListener(() => ClosePanel(panelE));
        closeButtonM.onClick.AddListener(() => ClosePanel(panelM));
        closeButtonG.onClick.AddListener(() => ClosePanel(panelG));
        closeErrorButton.onClick.AddListener(() => errorPanel.SetActive(false));
    }

    public void ShowCard()
    {
        string cardType = textDisplay.text.Trim();

        switch (cardType)
        {
            case "S":
                panelS.SetActive(true);
                StartCoroutine(FetchCardData("社會", panelS, cardNameTextS, cardContentTextS, cardMoneyTextS, cardESGTextS, cardKnowTextS, cardTypeTextS));
                break;
            case "E":
                panelE.SetActive(true);
                StartCoroutine(FetchCardData("法規", panelE, cardNameTextE, cardContentTextE, cardMoneyTextE, cardESGTextE, cardKnowTextE, cardTypeTextE));
                break;
            case "G":
                panelG.SetActive(true);
                StartCoroutine(FetchCardData("創新", panelG, cardNameTextG, cardContentTextG, cardMoneyTextG, cardESGTextG, cardKnowTextG, cardTypeTextG));
                break;
            case "M":
                panelM.SetActive(true);
                cardNameTextM.text = "金錢獎勳";
                break;
            default:
                Debug.LogError("無效的類型: " + cardType);
                break;
        }
    }

    private IEnumerator FetchCardData(string dataType, GameObject panel, TextMeshProUGUI nameText, TextMeshProUGUI contentText, TextMeshProUGUI moneyText, TextMeshProUGUI esgText, TextMeshProUGUI knowText, TextMeshProUGUI typeText)
    {
        string url = $"https://134.208.97.162:7285/api/Card/random/{dataType}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            errorText.text = "無法連接到伺服器，請稍後再試！";
            errorPanel.SetActive(true);
            yield break;
        }

        string json = request.downloadHandler.text;
        currentCard = JsonUtility.FromJson<CardData>(json);

        nameText.text = currentCard.cardname;
        contentText.text = "內容: " + currentCard.cardin;
        moneyText.text = "金錢: " + currentCard.cardmoney;
        esgText.text = "ESG: " + currentCard.cardesg;
        knowText.text = "背景知識: " + currentCard.cardknow;
        typeText.text = "種類: " + currentCard.cardtype;

        panelC.SetActive(true);
        chooseText1.text = currentCard.choose1;
        chooseText2.text = currentCard.choose2;
        chooseText3.text = currentCard.choose3;
        chooseText4.text = currentCard.choose4;

        float originalMoney = currentCard.cardmoney;
        float originalESG = currentCard.cardesg;

        chooseButton1.onClick.RemoveAllListeners();
        chooseButton1.onClick.AddListener(() => ApplyChoice(1, originalMoney, originalESG, contentText));

        chooseButton2.onClick.RemoveAllListeners();
        chooseButton2.onClick.AddListener(() => ApplyChoice(2, originalMoney, originalESG, contentText));

        chooseButton3.onClick.RemoveAllListeners();
        chooseButton3.onClick.AddListener(() => ApplyChoice(3, originalMoney, originalESG, contentText));

        chooseButton4.onClick.RemoveAllListeners();
        chooseButton4.onClick.AddListener(() => ApplyChoice(4, originalMoney, originalESG, contentText));

        if (NetworkClient.Instance != null && !string.IsNullOrEmpty(NetworkClient.Instance.playerName))
        {
            string cardMessage = $"CARD:{NetworkClient.Instance.playerName},{currentCard.cardname},{currentCard.cardin},{currentCard.cardmoney},{currentCard.cardesg},{currentCard.cardknow},{currentCard.cardtype},{currentCard.choose1},{currentCard.choose2},{currentCard.choose3},{currentCard.choose4}";
            NetworkClient.Instance.SendMessageToServer(cardMessage);
        }
    }

    private void ApplyChoice(int choice, float originalMoney, float originalESG, TextMeshProUGUI contentText)
    {
        switch (choice)
        {
            case 1:
                modifiedESG = originalESG * -0.5f;
                modifiedMoney = originalMoney;
                break;
            case 2:
                modifiedMoney = originalMoney * 0.7f;
                modifiedESG = originalESG * 1.5f;
                break;
            case 3:
                modifiedESG = originalESG * 1.5f;
                modifiedMoney = originalMoney;
                break;
            case 4:
                modifiedMoney = originalMoney * 0.9f;
                modifiedESG = originalESG * 1.2f;
                break;
        }

        contentText.text += $"\n\n[選擇結果]\n金錢變化後: {modifiedMoney:F1}\nESG變化後: {modifiedESG:F1}";
        panelC.SetActive(false);
        StartCoroutine(DelayedTurnEnd());
    }

    private IEnumerator DelayedTurnEnd()
    {
        yield return new WaitForSeconds(3f);
        TurnManager.Instance.EndTurn();
    }

    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}

[System.Serializable]
public class CardData
{
    public int cardid;
    public string cardname;
    public string cardin;
    public float cardmoney;
    public float cardesg;
    public string cardknow;
    public string cardtype;
    public string choose1;
    public string choose2;
    public string choose3;
    public string choose4;
}

public class BypassCertificate : UnityEngine.Networking.CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
