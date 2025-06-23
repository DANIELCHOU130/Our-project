using UnityEngine;
using System.Collections;

public class dicechange : MonoBehaviour
{
    public Sprite[] rollingFaces; // 滾動時使用的圖片 (9 張滾動動畫圖片)
    public Sprite[] resultFaces;  // 最終結果圖片 (6 張結果骰子面)

    public move moveScript;

    private int currentIndex = 0;
    private bool isRolling = false;
    private Coroutine rollingCoroutine;

    void Start()
    {
        HideAllDiceParts(); // 隱藏剛進入時的骰子圖片
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            if (!TurnManager.Instance.IsMyTurn())
            {
                Debug.Log("不是你的回合，不能擲骰！");
                return;
            }

            RollDiceWithTimer();
        }
    }

    public void RollDiceWithTimer()
    {
        if (!isRolling)
        {
            rollingCoroutine = StartCoroutine(RollDice());
        }
    }

    public void RollDiceAuto()
    {
        if (!isRolling)
        {
            Debug.Log("自動擲骰啟動！");
            rollingCoroutine = StartCoroutine(RollDice());
        }
    }

    IEnumerator RollDice()
    {
        if (rollingFaces == null || rollingFaces.Length == 0) yield break;

        isRolling = true;

        float rollDuration = 2.0f;
        float fixedSpeed = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < rollDuration)
        {
            ShowNextRollingFace();
            elapsedTime += fixedSpeed;
            yield return new WaitForSeconds(fixedSpeed);
        }

        StopRolling();
    }

    void StopRolling()
    {
        if (resultFaces == null || resultFaces.Length == 0) return;

        isRolling = false;
        int finalIndex = Random.Range(0, resultFaces.Length);
        ShowResultFace(finalIndex);

        if (moveScript != null)
        {
            moveScript.dicenumber = finalIndex + 1;
            StartCoroutine(moveScript.MoveSteps(moveScript.dicenumber));
        }
    }

    void ShowNextRollingFace()
    {
        if (rollingFaces == null || rollingFaces.Length == 0) return;

        int randomIndex = Random.Range(0, rollingFaces.Length);
        GetComponent<SpriteRenderer>().sprite = rollingFaces[randomIndex];

        float randomRotation = Random.Range(-15f, 15f);
        transform.rotation = Quaternion.Euler(0, 0, randomRotation);

        float randomScale = Random.Range(0.9f, 1.1f);
        transform.localScale = new Vector3(randomScale, randomScale, 1);
    }

    void ShowResultFace(int index)
    {
        if (resultFaces == null || index < 0 || index >= resultFaces.Length) return;

        GetComponent<SpriteRenderer>().sprite = resultFaces[index];
    }

    void HideAllDiceParts()
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }
}
