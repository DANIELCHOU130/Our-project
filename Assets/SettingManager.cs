using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("UI ���")]
    public Slider brightnessSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button logoutButton;

    [Header("���ĻP����")]
    public AudioSource musicSource;  // �I������
    public AudioSource sfxSource;    // ���Ĩӷ�
    public AudioClip clickSFX;       // �I������

    [Header("�e���G�׾B�n")]
    public Image screenOverlay;

    // �I�����ħN�o
    private float lastClickTime = 0f;
    private float clickCooldown = 0.1f;

    private void Start()
    {
        // �]�w Slider ��l��
        brightnessSlider.value = 1f;
        musicVolumeSlider.value = musicSource != null ? musicSource.volume : 0.5f;
        sfxVolumeSlider.value = sfxSource != null ? sfxSource.volume : 0.5f;

        // ���U�ƥ�
        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
        musicVolumeSlider.onValueChanged.AddListener(AdjustMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(AdjustSFXVolume);
        logoutButton.onClick.AddListener(Logout);

        // ����I������
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
        Debug.Log("�n�X��...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}
