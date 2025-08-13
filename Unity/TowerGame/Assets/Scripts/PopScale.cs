using System.Collections;
using UnityEngine;

public class ScalePop : MonoBehaviour
{
    [Header("Pop amount (e.g. 0.10 = +10%)")]
    [Range(0f, 1f)] public float popPercent = 0.10f;

    [Header("How long the whole pop takes (up+down)")]
    public float popTime = 0.12f;

    Coroutine popRoutine;

    // Call this from code or hook it to a UI Button OnClick
    public void Pop()
    {
        if (popRoutine != null) StopCoroutine(popRoutine);
        popRoutine = StartCoroutine(PopCo());
    }

    IEnumerator PopCo()
    {
        Vector3 start = transform.localScale;
        Vector3 peak  = start * (1f + popPercent);

        float t = 0f;
        while (t < popTime)
        {
            float a = t / popTime;
            // Sine 0→1→0 curve: hits peak in the middle, returns to start
            float k = Mathf.Sin(a * Mathf.PI);
            transform.localScale = Vector3.LerpUnclamped(start, peak, k);

            t += Time.unscaledDeltaTime; // ignores timescale so it always pops nicely
            yield return null;
        }

        transform.localScale = start; // snap back just in case
        popRoutine = null;
    }

    // Optional: call with a custom percent from code
    public void PopPercent(float percent)
    {
        popPercent = percent;
        Pop();
    }
}