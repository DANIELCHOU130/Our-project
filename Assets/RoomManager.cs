using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class RoomManagerAPI : MonoBehaviour
{
    public TMP_InputField roomIdInputField;
    public TMP_Dropdown playerDropdown;
    public TMP_Text playerDataText;
    public Button resetButton;
    public Button roomGetButton;

    private List<int> playerIds = new List<int>();
    private const string baseApiUrl = "http://134.208.97.162:5000/it/ROOMAPI/api/game";

    void Start()
    {
        if (roomGetButton != null)
        {
            roomGetButton.onClick.AddListener(() =>
            {
                string roomId = roomIdInputField?.text?.Trim();
                if (!string.IsNullOrEmpty(roomId))
                    StartCoroutine(FetchRoomPlayers(roomId));
                else
                    playerDataText.text = "請輸入房間 ID！";
            });
        }
        else
        {
            Debug.LogWarning("roomGetButton 尚未綁定！");
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(() =>
            {
                string roomId = roomIdInputField?.text?.Trim();
                if (!string.IsNullOrEmpty(roomId))
                    StartCoroutine(ResetPlayersData(roomId));
                else
                    playerDataText.text = "請輸入房間 ID！";
            });
        }
        else
        {
            Debug.LogWarning("resetButton 尚未綁定！");
        }

        if (playerDropdown != null)
        {
            playerDropdown.onValueChanged.AddListener(index =>
            {
                if (index >= 0 && index < playerIds.Count)
                    StartCoroutine(FetchPlayerData(playerIds[index]));
            });
        }
        else
        {
            Debug.LogWarning("playerDropdown 尚未綁定！");
        }

        if (playerDataText != null)
            playerDataText.text = "請輸入房間 ID 並點擊查詢。";
    }

    // 查詢房間玩家列表
    IEnumerator FetchRoomPlayers(string roomId)
    {
        string url = $"{baseApiUrl}/status/{roomId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                playerDataText.text = $"取得玩家列表失敗: {request.error}";
                Debug.LogError(request.error);
            }
            else
            {
                string raw = request.downloadHandler.text.Trim(); // 例如 "WAITING,3|4|5"
                string[] parts = raw.Split(',');
                if (parts.Length != 2)
                {
                    playerDataText.text = "API 回傳格式錯誤！";
                    Debug.LogError("API 回傳格式錯誤：" + raw);
                    yield break;
                }

                string[] playerIdStrings = parts[1].Split('|');
                playerIds.Clear();
                List<string> options = new List<string>();

                foreach (var pidStr in playerIdStrings)
                {
                    if (int.TryParse(pidStr, out int pid))
                    {
                        playerIds.Add(pid);
                        options.Add($"玩家 {pid}");
                    }
                }

                playerDropdown.ClearOptions();

                if (playerIds.Count == 0)
                {
                    options.Add("無玩家數據");
                    playerDataText.text = "此房間無玩家數據";
                }
                else
                {
                    StartCoroutine(FetchPlayerData(playerIds[0]));
                }

                playerDropdown.AddOptions(options);
                playerDropdown.value = 0;
                playerDropdown.RefreshShownValue();
            }
        }
    }

    // 取得玩家詳細資料
    IEnumerator FetchPlayerData(int playerId)
    {
        string url = $"{baseApiUrl}/player/{playerId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                playerDataText.text = $"取得玩家資料失敗: {request.error}";
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                try
                {
                    PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                    playerDataText.text = $"金錢: {data.gamermoney}\nESG: {data.gameresg}";
                }
                catch (Exception e)
                {
                    playerDataText.text = "解析玩家資料失敗";
                    Debug.LogError("JSON 解析錯誤：" + e.Message + "\n回傳內容：" + json);
                }
            }
        }
    }

    // 重設玩家資料
    IEnumerator ResetPlayersData(string roomId)
    {
        string url = $"{baseApiUrl}/reset/{roomId}";
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                playerDataText.text = $"重置失敗: {request.error}";
                Debug.LogError(request.error);
            }
            else
            {
                playerDataText.text = "所有玩家資料已重置！";
                StartCoroutine(FetchRoomPlayers(roomId));
            }
        }
    }

    [Serializable]
    private class PlayerData
    {
        public float gamermoney;
        public string gameresg;
    }
}
