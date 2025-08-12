using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [Header("Press Settings")]
    [Range(0f, 2f)]
    public float pressedScaleMultiplier = 0.9f; // 0.9 means 90% of original
    public float scaleSpeed = 10f;

    [Header("Events")]
    public UnityEvent onClick;

    private Vector3 originalScale;
    private Vector3 pressedScale; 
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
        pressedScale = originalScale * pressedScaleMultiplier; // relative scale
    }

    void OnMouseDown()
    {
        isPressed = true;
        StopAllCoroutines();
        StartCoroutine(ScaleTo(pressedScale));
    }

    void OnMouseUp()
    {
        if (isPressed)
        {
            isPressed = false;
            StopAllCoroutines();
            StartCoroutine(ScaleTo(originalScale));

            onClick?.Invoke();
        }
    }

    void OnMouseExit()
    {
        if (isPressed)
        {
            isPressed = false;
            StopAllCoroutines();
            StartCoroutine(ScaleTo(originalScale));
        }
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * scaleSpeed);
            yield return null;
        }
        transform.localScale = target;
    }
}