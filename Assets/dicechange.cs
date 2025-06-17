using UnityEngine;
using System.Collections;

public class dicechange : MonoBehaviour
{
    // 9 張滾動用圖片 + 6 張結果圖片
    public Sprite[] rollingFaces; // 滾動時使用的圖片 (9 張滾動動畫圖片)
    public Sprite[] resultFaces;  // 最終結果圖片 (6 張結果骰子面)

    public move moveScript;

    private int currentIndex = 0;
    private bool isRolling = false;  // 是否正在滾動
    private Coroutine rollingCoroutine;

    public bool IsRolling => isRolling;

    void Start()
    {
        HideAllDiceParts(); // 隱藏剛進入時的骰子圖片
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && TurnManager.Instance.IsMyTurn())
        {
            rollingCoroutine = StartCoroutine(RollDice());
        }
    }

<<<<<<< HEAD
    public IEnumerator RollDice()
=======
    // 修改的部分：滾動階段只使用 rollingFaces (動畫圖片)
    IEnumerator RollDice()
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    {
        if (rollingFaces == null || rollingFaces.Length == 0) yield break;

        isRolling = true; // 表示正在滾動

        float rollDuration = 2.0f; // 滾動動畫總持續時間
        float fixedSpeed = 0.1f;  // 每次切換圖片的固定間隔時間

        float elapsedTime = 0f;   // 記錄滾動畫持續時間

        while (elapsedTime < rollDuration)
        {
            ShowNextRollingFace(); // 滾動畫面更新 (隨機切換、旋轉、縮放)
            elapsedTime += fixedSpeed; // 根據固定速度累加時間
            yield return new WaitForSeconds(fixedSpeed); // 等待固定時間間隔後切換圖片
        }

        StopRolling(); // 停止滾動，展示最終結果
    }



    // 修改的部分：停止滾動，選定最終結果
    void StopRolling()
    {
        if (resultFaces == null || resultFaces.Length == 0) return;

<<<<<<< HEAD
        isRolling = false;
        int finalIndex = Random.Range(0, diceFaces.Length);
        ShowPart(finalIndex);
=======
        isRolling = false; // 解鎖滾動
        int finalIndex = Random.Range(0, resultFaces.Length); // 取得隨機結果

        ShowResultFace(finalIndex); // 顯示最終結果圖片
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34

        // 傳遞骰子點數給 moveScript
        if (moveScript != null)
        {
            moveScript.dicenumber = finalIndex + 1; // 因為骰子點數是從 1 開始的
            StartCoroutine(moveScript.MoveSteps(moveScript.dicenumber)); // 執行角色移動
        }
    }

    // 顯示下一張滾動動畫用圖片 (從 rollingFaces 陣列中取)
    void ShowNextRollingFace()
    {
        if (rollingFaces == null || rollingFaces.Length == 0) return;

        // **隨機切換圖片**
        int randomIndex = Random.Range(0, rollingFaces.Length);
        GetComponent<SpriteRenderer>().sprite = rollingFaces[randomIndex];

        // **隨機旋轉**
        float randomRotation = Random.Range(-15f, 15f);
        transform.rotation = Quaternion.Euler(0, 0, randomRotation);

        // **隨機縮放**
        float randomScale = Random.Range(0.9f, 1.1f);
        transform.localScale = new Vector3(randomScale, randomScale, 1);
    }


    // 顯示最終結果圖片 (從 resultFaces 陣列中取)
    void ShowResultFace(int index)
    {
        if (resultFaces == null || index < 0 || index >= resultFaces.Length) return;

        GetComponent<SpriteRenderer>().sprite = resultFaces[index]; // 顯示正確結果圖片
    }

    // 隱藏骰子的所有部分 (此功能可根據需要擴展)
    void HideAllDiceParts()
    {
<<<<<<< HEAD
        // 可選：初始化時清空顯示
    }

    // ✅ 提供外部呼叫自動擲骰
    public void RollDiceExternally()
    {
        if (!isRolling)
        {
            rollingCoroutine = StartCoroutine(RollDice());
        }
=======
        GetComponent<SpriteRenderer>().sprite = null; // 遮罩圖片
>>>>>>> 22d1680faaedc46c26be3edcbe6ab6fcac0aef34
    }
}
