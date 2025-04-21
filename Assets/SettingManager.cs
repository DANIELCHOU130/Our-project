using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("UI 控制項")]
    public Slider brightnessSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button logoutButton;

    [Header("音效與音樂")]
    public AudioSource musicSource;  // 背景音樂
    public AudioSource sfxSource;    // 音效來源
    public AudioClip clickSFX;       // 點擊音效

    [Header("畫面亮度遮罩")]
    public Image screenOverlay;

    // 點擊音效冷卻
    private float lastClickTime = 0f;
    private float clickCooldown = 0.1f;

    private void Start()
    {
        // 設定 Slider 初始值
        brightnessSlider.value = 1f;
        musicVolumeSlider.value = musicSource != null ? musicSource.volume : 0.5f;
        sfxVolumeSlider.value = sfxSource != null ? sfxSource.volume : 0.5f;

        // 註冊事件
        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
        musicVolumeSlider.onValueChanged.AddListener(AdjustMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(AdjustSFXVolume);
        logoutButton.onClick.AddListener(Logout);

        // 播放背景音樂
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void AdjustBrightness(float value)
    {
        if (screenOverlay != null)
        {
            Color overlayColor = screenOverlay.color;
            overlayColor.a = 1f - value;
            screenOverlay.color = overlayColor;
        }
    }

    public void AdjustMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }

    public void AdjustSFXVolume(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }
    }

    public void PlayClickSound()
    {
        if (sfxSource != null && clickSFX != null)
        {
            if (Time.time - lastClickTime > clickCooldown)
            {
                sfxSource.PlayOneShot(clickSFX);
                lastClickTime = Time.time;
            }
        }
    }

    public void Logout()
    {
        Debug.Log("登出中...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}
