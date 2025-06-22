using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InformationManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject informationPanel;
    public GameObject passwordPanel;
    public GameObject editPanel;

    [Header("UI Components")]
    public TextMeshProUGUI infoText;
    public TMP_InputField passwordInputField;
    public TMP_InputField editInputField;
    public TextMeshProUGUI errorTextPW;

    [Header("Buttons")]
    public Button developerButton;
    public Button submitPasswordButton;
    public Button saveEditButton;
    public Button backButtonPW;
    public Button backButtonEdit;

    private const string CorrectPassword = "changetext14";
    private const string SavedInfoKey = "SavedInfoText";

    void Start()
    {
        passwordPanel.SetActive(false);
        editPanel.SetActive(false);
        errorTextPW.text = "";

        developerButton.onClick.AddListener(OpenPasswordPanel);
        submitPasswordButton.onClick.AddListener(CheckPassword);
        saveEditButton.onClick.AddListener(SaveNewText);
        backButtonPW.onClick.AddListener(ClosePasswordPanel);
        backButtonEdit.onClick.AddListener(CloseEditPanel);

        if (PlayerPrefs.HasKey(SavedInfoKey))
        {
            infoText.text = PlayerPrefs.GetString(SavedInfoKey);
        }
    }

    private void OpenPasswordPanel()
    {
        passwordPanel.SetActive(true);
        errorTextPW.text = "";
    }

    private void CheckPassword()
    {
        if (passwordInputField.text == CorrectPassword)
        {
            passwordPanel.SetActive(false);
            editPanel.SetActive(true);
            editInputField.text = infoText.text;
        }
        else
        {
            errorTextPW.text = "±K½X¿ù»~¡A½Ð­«¸Õ¡C";
        }
        passwordInputField.text = "";
    }

    private void SaveNewText()
    {
        string newText = editInputField.text;
        infoText.text = newText;
        PlayerPrefs.SetString(SavedInfoKey, newText);
        PlayerPrefs.Save();
        editPanel.SetActive(false);
    }

    private void ClosePasswordPanel()
    {
        passwordPanel.SetActive(false);
        passwordInputField.text = "";
        errorTextPW.text = "";
    }

    private void CloseEditPanel()
    {
        editPanel.SetActive(false);
    }
}
