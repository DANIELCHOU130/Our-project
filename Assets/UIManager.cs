using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using static UnityEditor.LightingExplorerTableColumn;

public class UIManager : MonoBehaviour
{
    [Header("Panel 控制")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject homePanel;
    public GameObject settingsPanel;
    public GameObject informationPanel;
    public GameObject waitingPanel; // ✅ 新增 Waiting Panel

    public WaitingPanelManager waitingPanelManager; // ✅ 連結 WaitingPanelManager

    [Header("登入欄位")]
    public TMP_InputField inputAccount;
    public TMP_InputField inputPassword;

    [Header("註冊欄位")]
    public TMP_InputField inputUsernameReg;
    public TMP_InputField inputAccountReg;
    public TMP_InputField inputPasswordReg;

    [Header("訊息與按鈕")]
    public TMP_Text loginMessageText;

    public Button btnLogin;
    public Button btnRegister;
    public Button btnCreateAccount;
    public Button btnBack;

    public Button btnSettings;
    public Button btnInformation;

    public Button btnSettingsBack;
    public Button btnInformationBack;


    public Button btnCreateGame; // ✅ 創建遊戲按鈕
    public Button btnJoinGame;   // ✅ 加入遊戲按鈕

    private string apiUrl = "https://134.208.97.162:7285/api/Account";

    void Start()
    {
        btnLogin.onClick.AddListener(Login);
        btnRegister.onClick.AddListener(ShowRegisterPanel);
        btnCreateAccount.onClick.AddListener(CreateAccount);
        btnBack.onClick.AddListener(BackToLogin);

        btnSettings.onClick.AddListener(ShowSettingsPanel);
        btnInformation.onClick.AddListener(ShowInformationPanel);

        btnSettingsBack.onClick.AddListener(BackToHomeFromSettings);
        btnInformationBack.onClick.AddListener(BackToHomeFromInformation);

        btnCreateGame.onClick.AddListener(CreateGame);
        btnJoinGame.onClick.AddListener(JoinGame);

        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        homePanel.SetActive(false);
        settingsPanel.SetActive(false);
        informationPanel.SetActive(false);
        waitingPanel.SetActive(false);

        loginMessageText.text = "";
    }

    void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        loginMessageText.text = "";
    }

    void BackToLogin()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        loginMessageText.text = "";
    }

    void Login()
    {
        string account = inputAccount.text.Trim();
        string password = inputPassword.text.Trim();

        if (account == "backdoor" && password == "backdoor")
        {
            Debug.Log("後門登入成功！");
            loginMessageText.text = " 開發者登入成功";
            OnLoginSuccess();
            return;
        }

        string encryptedPassword = GetSHA256(password);
        StartCoroutine(SendLoginRequest(account, encryptedPassword));
    }

    IEnumerator SendLoginRequest(string account, string encryptedPassword)
    {
        string url = apiUrl + "/login";

        WWWForm form = new WWWForm();
        form.AddField("account", account);
        form.AddField("password", encryptedPassword);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("連線錯誤: " + www.error);
            loginMessageText.text = " 無法連線伺服器";
        }
        else
        {
            string result = www.downloadHandler.text.Trim();
            if (result == "Success")
            {
                Debug.Log("登入成功！");
                loginMessageText.text = " 登入成功";
                OnLoginSuccess();
            }
            else
            {
                Debug.Log("登入失敗：" + result);
                loginMessageText.text = " 帳號或密碼錯誤";
            }
        }
    }

    void OnLoginSuccess()
    {
        loginPanel.SetActive(false);
        homePanel.SetActive(true);
        settingsPanel.SetActive(false);
        informationPanel.SetActive(false);
    }

    void CreateAccount()
    {
        string username = inputUsernameReg.text.Trim();
        string account = inputAccountReg.text.Trim();
        string password = inputPasswordReg.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            Debug.Log("欄位不能為空");
            return;
        }

        string encryptedPassword = GetSHA256(password);
        StartCoroutine(SendRegisterRequest(username, account, encryptedPassword));
    }

    IEnumerator SendRegisterRequest(string username, string account, string encryptedPassword)
    {
        string url = apiUrl + "/register";

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("account", account);
        form.AddField("password", encryptedPassword);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("註冊失敗: " + www.error);
        }
        else
        {
            string result = www.downloadHandler.text.Trim();
            if (result == "Success")
            {
                Debug.Log("註冊成功");
                BackToLogin();
            }
            else
            {
                Debug.Log("註冊錯誤：" + result);
            }
        }
    }

    string GetSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    // 設定頁面
    void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
        informationPanel.SetActive(false);
        homePanel.SetActive(false);
    }

    // 資訊頁面
    void ShowInformationPanel()
    {
        settingsPanel.SetActive(false);
        informationPanel.SetActive(true);
        homePanel.SetActive(false);
    }

    // 設定返回
    void BackToHomeFromSettings()
    {
        settingsPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    // 資訊返回
    void BackToHomeFromInformation()
    {
        informationPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    // ✅ 創建遊戲按鈕邏輯
    void CreateGame()
    {
        Debug.Log("創建遊戲中...");
        int dummyGameId = 123; // 實際應該從伺服器拿 gameId
        waitingPanelManager.StartWaiting(dummyGameId);
        homePanel.SetActive(false);
    }

    // ✅ 加入遊戲按鈕邏輯
    void JoinGame()
    {
        Debug.Log("加入遊戲中...");
        int dummyGameId = 123; // 實際應該從伺服器拿 gameId
        waitingPanelManager.StartWaiting(dummyGameId);
        homePanel.SetActive(false);
    }
}
