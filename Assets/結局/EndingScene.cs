using System.Collections;
using UnityEngine;
using TMPro;

public class EndingScene : MonoBehaviour
{
    public TextMeshProUGUI titleText;  //標題
    public TextMeshProUGUI bodyText;  // 旁白
    public AudioSource bgmSource;     // 音樂
    public float typingSpeed = 0.05f; // 打字速度

    private string[] endingLines;
    private int currentLine = 0;
    private bool isTyping = false;

    //顯示結局
    public void ShowEnding(EndingData data)
    {
        titleText.text = data.endingTitle;
        endingLines = data.endingLines;
        bgmSource.clip = data.bgmClip;
        bgmSource.Play();
        currentLine = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        bodyText.text = "";

        foreach (char c in endingLines[currentLine])
        {
            bodyText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                bodyText.text = endingLines[currentLine];
                isTyping = false;
            }
            else
            {
                currentLine++;
                if (currentLine < endingLines.Length)
                    StartCoroutine(TypeLine());
                else
                    Debug.Log("結局播放完畢");
            }
        }
    }
}
