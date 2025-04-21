using UnityEngine;

public class UIClickSoundPlayer : MonoBehaviour
{
    public SettingManager settingManager;

    private float lastClickTime = 0f;
    private float clickCooldown = 0.1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime > clickCooldown)
            {
                settingManager.PlayClickSound();
                lastClickTime = Time.time;
            }
        }
    }
}
