using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    private string connectionString = "Data Source=192.168.1.175\\\\SQLEXPRESS03;Initial Catalog=ESGGAMEDB;User ID=Dan;Password=NewStrongP@ssword2024;TrustServerCertificate=True;Connect Timeout=30";

    public TMP_InputField roomIdInputField; // �ж� ID ����J��
    public TMP_Dropdown playerDropdown; // ��ܪ��a���U�Կ��
    public TMP_Text playerDataText; // ��ܪ��a�ƾڪ��奻
    public Button resetButton;
    public Button roomGetButton;

    private List<int> playerIds = new List<int>();

    void Start()
    {
        roomGetButton.onClick.AddListener(FetchRoomPlayers);
        resetButton.onClick.AddListener(SetPlayersDataToZero);

        playerDropdown.ClearOptions();
        playerDataText.text = "�п�ܪ��a�H��ܼƾڡC";

        // �]�w�U�Կ�檺�ƥ��ť��
        playerDropdown.onValueChanged.AddListener(delegate { DisplayPlayerData(); });
    }

    // �d�߬Y�өж������a
    public void FetchRoomPlayers()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("�п�J�ж� ID�I");
            return;
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT gamerid FROM dbo.nowgamedata WHERE boardid = @roomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                SqlDataReader reader = cmd.ExecuteReader();

                playerDropdown.ClearOptions(); // �C����s���M�ſﶵ
                playerIds.Clear();

                List<string> options = new List<string>();
                while (reader.Read())
                {
                    int playerId = reader.GetInt32(0); // ���o���a ID
                    playerIds.Add(playerId);
                    options.Add($"���a {playerId}");
                }

                if (options.Count == 0)
                {
                    // �p�G�L���a�ƾ�
                    options.Add("�L���a�ƾ�");
                    playerDataText.text = "�d�ߵ��G�G���ж��L���a�ƾڰO��";
                }

                playerDropdown.AddOptions(options);
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL ���~ - {ex.Message}\n{ex.StackTrace}");
        }
    }

    // ��ܿ襤���a���ƾ�
    public void DisplayPlayerData()
    {
        int selectedIndex = playerDropdown.value;

        // �p�G�ﶵ�L�ĩεL���a�ƾڡA�h���X
        if (playerIds.Count == 0 || selectedIndex <= 0)
        {
            playerDataText.text = "�п�ܦ��Ī����a����ܼƾڡC";
            return;
        }

        if (selectedIndex < 0 || selectedIndex >= playerIds.Count)
        {
            Debug.LogError("�п�ܦ��Ī����a�I");
            return;
        }

        int playerId = playerIds[selectedIndex];

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT gamermoney, gameresg FROM dbo.nowgamedata WHERE gamerid = @playerId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@playerId", playerId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    float money = reader.IsDBNull(0) ? 0 : reader.GetFloat(0); // �q�ƾڮw��Ū�������ƾڡ]�קK�ŭȿ��~�^
                    string esg = reader.IsDBNull(1) ? "�L" : reader.GetString(1); // �B�z�i�ର NULL �� ESG �ƾ�
                    playerDataText.text = $"����: {money}\nESG: {esg}";
                }
                else
                {
                    playerDataText.text = "�����Ӫ��a���ƾڡC";
                }
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL ���~: {ex.Message}");
        }
    }

    // ���m���a�ƾڥ\��
    public void SetPlayersDataToZero()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("�п�J�ж� ID�I");
            return;
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE dbo.nowgamedata SET gamermoney = 0, gameresg = '0' WHERE boardid = @roomId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roomId", roomId);

                int rowsAffected = cmd.ExecuteNonQuery();
                Debug.Log($"�w�N {rowsAffected} �쪱�a�ƾڭ��m�� 0�C");

                // �M�żƾ���ܮ�
                playerDataText.text = "�Ҧ����a�ƾڤw���m�I";

                // ��s�U�Ԯؤ��e
                FetchRoomPlayers();
            }
        }
        catch (SqlException ex)
        {
            Debug.LogError($"SQL ���~: {ex.Message}");
        }
    }
}
