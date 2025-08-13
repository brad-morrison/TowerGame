using UnityEngine;
using System.Collections;

public class Notification : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] tokens;

    [Header("Movement Settings")]
    public float popDownDistance = 0.1f;   // positive number; we move DOWN by this much
    public float moveDuration = 0.2f;
    public float visibleTime = 3f;

    private Vector3 startLocal;
    private Vector3 downLocal;
    private bool isAnimating;

    void Start()
    {
        // 1) Use LOCAL, not world
        startLocal = transform.localPosition;
        // 2) Down is -Y
        downLocal  = startLocal + Vector3.down * Mathf.Abs(popDownDistance);

        // brand stuff unchanged
        switch (gameManager.activeBrand)
        {
            case 1: tokens[0].SetActive(true); break;
            case 2: tokens[1].SetActive(true); break;
            case 3: tokens[2].SetActive(true); break;
            default: tokens[0].SetActive(true); break;
        }
    }

    public void PopNotification()
    {
        if (!isAnimating) StartCoroutine(PopupSequence());
    }

    IEnumerator PopupSequence()
    {
        isAnimating = true;

        // Pop in (down)
        yield return MoveTo(downLocal, moveDuration);

        // Wait
        yield return new WaitForSeconds(visibleTime);

        // Pop out (up)
        yield return MoveTo(startLocal, moveDuration);

        isAnimating = false;
    }

    IEnumerator MoveTo(Vector3 targetLocal, float duration)
    {
        Vector3 from = transform.localPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            transform.localPosition = Vector3.Lerp(from, targetLocal, u);
            yield return null;
        }

        transform.localPosition = targetLocal;
    }
}