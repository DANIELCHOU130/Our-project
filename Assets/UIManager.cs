
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

public class UIManager : MonoBehaviour
{
    [Header("Panel 控制")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject homePanel;

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

    // 請改為你的 Web API 位置
    private string apiUrl = "http://localhost:5000/api/Account";

    void Start()
    {
        // 按鈕綁定事件
        btnLogin.onClick.AddListener(Login);
        btnRegister.onClick.AddListener(ShowRegisterPanel);
        btnCreateAccount.onClick.AddListener(CreateAccount);
        btnBack.onClick.AddListener(BackToLogin);

        // 初始面板狀態
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        homePanel.SetActive(false);
        loginMessageText.text = "";
    }

    // 顯示註冊畫面
    void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        loginMessageText.text = "";
    }

    // 返回登入畫面
    void BackToLogin()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        loginMessageText.text = "";
    }

    // 登入流程
    void Login()
    {
        string account = inputAccount.text.Trim();
        string password = inputPassword.text.Trim();

        // ✅ 開發後門
        if (account == "backdoor" && password == "backdoor")
        {
            Debug.Log("後門登入成功！");
            loginMessageText.text = " 開發者登入成功";
            OnLoginSuccess();
            return;
        }

        // 正常加密登入流程
        string encryptedPassword = GetSHA256(password);
        StartCoroutine(SendLoginRequest(account, encryptedPassword));
    }

    // 傳送登入請求
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

    // 登入成功後切換面板
    void OnLoginSuccess()
    {
        loginPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    // 註冊流程
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

    // 傳送註冊請求
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

    // 密碼加密 SHA256
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
}
