
using UnityEngine;
using TMPro;

public class CameraFollow : MonoBehaviour
{
    [Header("跟隨設定")]
    public Transform target;
    public Vector3 offset = new(0, 10f, -10f);
    public float followSpeed = 5f;

    [Header("UI 顯示")]
    public TMP_Text viewModeText;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isFollowing = true;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        UpdateViewModeText();
    }

    void LateUpdate()
    {
        if (!isFollowing || target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    public void ToggleViewMode()
    {
        isFollowing = !isFollowing;
        if (!isFollowing)
            StartCoroutine(SmoothMoveTo(originalPosition, originalRotation));

        UpdateViewModeText();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (isFollowing && target != null)
            transform.position = target.position + offset;

        UpdateViewModeText();
    }

    private System.Collections.IEnumerator SmoothMoveTo(Vector3 targetPos, Quaternion targetRot)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void UpdateViewModeText()
    {
        if (viewModeText != null)
            viewModeText.text = isFollowing
                ? $"視角：跟隨 {(target != null ? target.name : "無")}"
                : "視角：全景模式";
    }
}
