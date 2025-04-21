using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InformationManager : MonoBehaviour
{
    public GameObject informationPanel;
    public GameObject passwordPanel;
    public GameObject editPanel;

    public TextMeshProUGUI infoText;
    public TMP_InputField passwordInputField;
    public TMP_InputField editInputField;
    public TextMeshProUGUI errorTextPW;

    public Button developerButton;
    public Button submitPasswordButton;
    public Button saveEditButton;
    public Button backButtonPW;
    public Button backButtonEdit;

    private string correctPassword = "changetext14";

    void Start()
    {
        passwordPanel.SetActive(false);
        editPanel.SetActive(false);
        errorTextPW.text = "";

        // 註冊按鈕事件
        developerButton.onClick.AddListener(OpenPasswordPanel);
        submitPasswordButton.onClick.AddListener(CheckPassword);
        saveEditButton.onClick.AddListener(SaveNewText);
        backButtonPW.onClick.AddListener(ClosePasswordPanel);
        backButtonEdit.onClick.AddListener(CloseEditPanel);

        // 載入儲存的留言
        if (PlayerPrefs.HasKey("SavedInfoText"))
        {
            infoText.text = PlayerPrefs.GetString("SavedInfoText");
        }
    }

    void OpenPasswordPanel()
    {
        passwordPanel.SetActive(true);
        errorTextPW.text = "";
    }

    void CheckPassword()
    {
        if (passwordInputField.text == correctPassword)
        {
            passwordPanel.SetActive(false);
            editPanel.SetActive(true);
            editInputField.text = infoText.text;
            passwordInputField.text = "";
            errorTextPW.text = "";
        }
        else
        {
            errorTextPW.text = "密碼錯誤，請重試。";
            passwordInputField.text = "";
        }
    }

    void SaveNewText()
    {
        infoText.text = editInputField.text;
        PlayerPrefs.SetString("SavedInfoText", editInputField.text); // 儲存
        PlayerPrefs.Save();
        editPanel.SetActive(false);
    }

    void ClosePasswordPanel()
    {
        passwordPanel.SetActive(false);
        passwordInputField.text = "";
        errorTextPW.text = "";
    }

    void CloseEditPanel()
    {
        editPanel.SetActive(false);
    }
}
