
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject mainPagePanel;
    public GameObject settingPanel;
    public GameObject informationPanel;

    public TMP_InputField inputAccount;
    public TMP_InputField inputPassword;
    public TMP_InputField inputUsernameReg;
    public TMP_InputField inputAccountReg;
    public TMP_InputField inputPasswordReg;

    public TMP_Text loginMessageText;

    public Button btnLogin;
    public Button btnRegister;
    public Button btnCreateAccount;
    public Button btnBack;
    public Button btnSettings;
    public Button btnInformation;

    private string apiBaseUrl = "https://localhost:7285"; 

    void Start()
    {
        btnLogin.onClick.AddListener(Login);
        btnRegister.onClick.AddListener(ShowRegisterPanel);
        btnCreateAccount.onClick.AddListener(Register);
        btnBack.onClick.AddListener(ShowLoginPanel);
        btnSettings.onClick.AddListener(ShowSettingPanel);
        btnInformation.onClick.AddListener(ShowInformationPanel);

        ShowLoginPanel();
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        mainPagePanel.SetActive(false);
        settingPanel.SetActive(false);
        informationPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void ShowMainPagePanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        mainPagePanel.SetActive(true);
        settingPanel.SetActive(false);
        informationPanel.SetActive(false);
    }

    public void ShowSettingPanel()
    {
        mainPagePanel.SetActive(false);
        settingPanel.SetActive(true);
        informationPanel.SetActive(false);
    }

    public void ShowInformationPanel()
    {
        mainPagePanel.SetActive(false);
        settingPanel.SetActive(false);
        informationPanel.SetActive(true);
    }

    public void Login()
    {
        string account = inputAccount.text.Trim();
        string password = inputPassword.text.Trim();
        string passwordHash = ComputeSha256Hash(password);

        StartCoroutine(LoginRequest(account, passwordHash));
    }

    IEnumerator LoginRequest(string account, string passwordHash)
    {
        string url = $"{apiBaseUrl}/api/auth/login";
        WWWForm form = new WWWForm();
        form.AddField("account", account);
        form.AddField("password", passwordHash);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                if (responseText.Contains("success"))
                {
                    Debug.Log("登入成功！");
                    loginMessageText.text = "登入成功！";
                    ShowMainPagePanel();
                }
                else
                {
                    Debug.Log("帳號或密碼錯誤！");
                    loginMessageText.text = "帳號或密碼錯誤！";
                }
            }
            else
            {
                Debug.LogError("登入請求失敗: " + www.error);
            }
        }
    }

    public void Register()
    {
        string username = inputUsernameReg.text.Trim();
        string account = inputAccountReg.text.Trim();
        string password = inputPasswordReg.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            Debug.Log("請填寫所有欄位！");
            return;
        }

        string passwordHash = ComputeSha256Hash(password);

        StartCoroutine(RegisterRequest(username, account, passwordHash));
    }

    IEnumerator RegisterRequest(string username, string account, string passwordHash)
    {
        string url = $"{apiBaseUrl}/api/auth/register";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("account", account);
        form.AddField("password", passwordHash);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                if (responseText.Contains("success"))
                {
                    Debug.Log("註冊成功！");
                    ShowLoginPanel();
                }
                else
                {
                    Debug.Log("註冊失敗！");
                }
            }
            else
            {
                Debug.LogError("註冊請求失敗: " + www.error);
            }
        }
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
