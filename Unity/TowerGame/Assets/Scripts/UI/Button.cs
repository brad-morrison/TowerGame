using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [Header("Press Settings")]
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);
    public float scaleSpeed = 10f;

    [Header("Events")]
    public UnityEvent onClick; // Assign your function in Inspector

    private Vector3 originalScale;
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseDown()
    {
        isPressed = true;
        StopAllCoroutines();
        StartCoroutine(ScaleTo(pressedScale));
    }

    void OnMouseUp()
    {
        if (isPressed) // still considered pressed
        {
            isPressed = false;
            StopAllCoroutines();
            StartCoroutine(ScaleTo(originalScale));

            onClick?.Invoke(); // trigger your function
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