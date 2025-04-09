using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;


public class CardDisplay : MonoBehaviour
{
    // ���O
    public GameObject panelS, panelE, panelM, panelG, panelC;

    // �U�۪� TextMeshProUGUI ����
    public TextMeshProUGUI cardNameTextS, cardContentTextS, cardMoneyTextS, cardESGTextS, cardKnowTextS, cardTypeTextS;
    public TextMeshProUGUI cardNameTextE, cardContentTextE, cardMoneyTextE, cardESGTextE, cardKnowTextE, cardTypeTextE;
    public TextMeshProUGUI cardNameTextM;
    public TextMeshProUGUI cardNameTextG, cardContentTextG, cardMoneyTextG, cardESGTextG, cardKnowTextG, cardTypeTextG;

    // Panel C ����
    public Button chooseButton1, chooseButton2, chooseButton3, chooseButton4;
    public TextMeshProUGUI chooseText1, chooseText2, chooseText3, chooseText4;

    // ��ܥd��������r
    public TMP_Text textDisplay;

    // �x�s�ܰʫ᪺��
    private float modifiedESG;
    private float modifiedMoney;

    // �������s
    public Button closeButtonS, closeButtonE, closeButtonM, closeButtonG;

    void Start()
    {
        panelS.SetActive(false);
        panelE.SetActive(false);
        panelM.SetActive(false);
        panelG.SetActive(false);
        panelC.SetActive(false);

        // ���U�������s�ƥ�
        closeButtonS.onClick.AddListener(() => ClosePanel(panelS));
        closeButtonE.onClick.AddListener(() => ClosePanel(panelE));
        closeButtonM.onClick.AddListener(() => ClosePanel(panelM));
        closeButtonG.onClick.AddListener(() => ClosePanel(panelG));
    }

    // ��ܹ������d��
    public void ShowCard()
    {
        string cardType = textDisplay.text.Trim();

        switch (cardType)
        {
            case "S":
                panelS.SetActive(true);
                StartCoroutine(FetchCardData("���|", panelS, cardNameTextS, cardContentTextS, cardMoneyTextS, cardESGTextS, cardKnowTextS, cardTypeTextS));
                break;
            case "E":
                panelE.SetActive(true);
                StartCoroutine(FetchCardData("�k�W", panelE, cardNameTextE, cardContentTextE, cardMoneyTextE, cardESGTextE, cardKnowTextE, cardTypeTextE));
                break;
            case "G":
                panelG.SetActive(true);
                StartCoroutine(FetchCardData("�зs", panelG, cardNameTextG, cardContentTextG, cardMoneyTextG, cardESGTextG, cardKnowTextG, cardTypeTextG));
                break;
            case "M":
                panelM.SetActive(true);
                cardNameTextM.text = "��������";
                break;
            default:
                Debug.LogError("�L�Ī�����: " + cardType);
                return;
        }
    }

    // �� UnityWebRequest �I�s API ���o�d�����
    private IEnumerator FetchCardData(string dataType, GameObject panel, TextMeshProUGUI nameText, TextMeshProUGUI contentText, TextMeshProUGUI moneyText, TextMeshProUGUI esgText, TextMeshProUGUI knowText, TextMeshProUGUI typeText)
    {
        string url = $"https://localhost:7285/api/Card/random/{dataType}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API �ШD����: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            CardData card = JsonUtility.FromJson<CardData>(json);

            // ��ܥd�����
            nameText.text = card.cardname;
            contentText.text = "���e: " + card.cardin;
            moneyText.text = "����: " + card.cardmoney;
            esgText.text = "ESG: " + card.cardesg;
            knowText.text = "�I������: " + card.cardknow;
            typeText.text = "����: " + card.cardtype;

            // ��� Panel C
            panelC.SetActive(true);

            chooseText1.text = card.choose1;
            chooseText2.text = card.choose2;
            chooseText3.text = card.choose3;
            chooseText4.text = card.choose4;

            float originalMoney = card.cardmoney;
            float originalESG = card.cardesg;

            // �j�w�ﶵ���s�ƥ�
            chooseButton1.onClick.RemoveAllListeners();
            chooseButton1.onClick.AddListener(() => ApplyChoice(1, originalMoney, originalESG, contentText));

            chooseButton2.onClick.RemoveAllListeners();
            chooseButton2.onClick.AddListener(() => ApplyChoice(2, originalMoney, originalESG, contentText));

            chooseButton3.onClick.RemoveAllListeners();
            chooseButton3.onClick.AddListener(() => ApplyChoice(3, originalMoney, originalESG, contentText));

            chooseButton4.onClick.RemoveAllListeners();
            chooseButton4.onClick.AddListener(() => ApplyChoice(4, originalMoney, originalESG, contentText));
        }
    }

    // �ھڿ�ܮM�ήĪG
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

        // ��ܵ��G
        contentText.text += $"\n\n[��ܵ��G]\n�����ܤƫ�: {modifiedMoney:F1}\nESG�ܤƫ�: {modifiedESG:F1}";

        // ������ܭ��O
        panelC.SetActive(false);
    }

    // �������O
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
        // �û���^ true�A���L SSL �����ˬd�]�u�����ն��q�^
        return true;
    }
}
