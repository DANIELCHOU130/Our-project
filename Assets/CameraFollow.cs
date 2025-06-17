
using UnityEngine;
using TMPro;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10f, -10f);
    public float followSpeed = 5f;
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
        if (isFollowing && target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }

    public void ToggleViewMode()
    {
        isFollowing = !isFollowing;
        if (!isFollowing)
        {
            StartCoroutine(SmoothMoveTo(originalPosition, originalRotation));
        }
        UpdateViewModeText();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (isFollowing && target != null)
        {
            transform.position = target.position + offset;
        }
        UpdateViewModeText();
    }

    private System.Collections.IEnumerator SmoothMoveTo(Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0f;
        float duration = 1f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void UpdateViewModeText()
    {
        if (viewModeText != null)
        {
            viewModeText.text = isFollowing ? $"視角：跟隨 {target?.name}" : "視角：全景模式";
        }
    }
}
