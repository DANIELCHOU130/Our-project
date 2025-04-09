using UnityEngine;
using System.Collections;

public class dicechange : MonoBehaviour
{
    public Sprite[] diceFaces; 
    public move moveScript; 

    private int currentIndex = 0;
    private bool isRolling = false;
    private Coroutine rollingCoroutine;

    void Start()
    {
        HideAllDiceParts(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            rollingCoroutine = StartCoroutine(RollDice());
        }
    }

    IEnumerator RollDice()
    {
        if (diceFaces == null || diceFaces.Length == 0) yield break;

        isRolling = true;
        float rollDuration = 2.0f; 
        float rollSpeed = 0.2f; 
        float elapsedTime = 0f;

        while (elapsedTime < rollDuration)
        {
            ShowNextPart();
            elapsedTime += rollSpeed;
            yield return new WaitForSeconds(rollSpeed);
        }

        StopRolling();
    }

    void StopRolling()
    {
        if (diceFaces == null || diceFaces.Length == 0) return;

        isRolling = false;

        int finalIndex = Random.Range(0, diceFaces.Length);
        ShowPart(finalIndex);

        if (moveScript != null)
        {
            moveScript.dicenumber = finalIndex + 1; 
            StartCoroutine(moveScript.MoveSteps(moveScript.dicenumber)); 
        }
    }

    void ShowNextPart()
    {
        if (diceFaces == null || diceFaces.Length == 0) return;

        currentIndex = (currentIndex + 1) % diceFaces.Length;
        GetComponent<SpriteRenderer>().sprite = diceFaces[currentIndex];
    }

    void ShowPart(int index)
    {
        if (diceFaces == null || index < 0 || index >= diceFaces.Length) return;
        GetComponent<SpriteRenderer>().sprite = diceFaces[index];
    }

    void HideAllDiceParts()
    {
        
    }
}
