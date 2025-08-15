using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Refs")]
    public GameManager gameManager;

    public GameObject coinCounterUI;

    [Header("Audio")]
    public AudioClip pickupSfx;
    public AudioClip crystalSfx;
    public AudioClip tokenSfx;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Crystal (win)")]
    public bool destroyCrystalOnPickup = true;

    [Header("Debug")]
    public bool logContacts = true;

    private bool _levelEnding = false;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other) return;

        if (!other.isTrigger) return;

        var otherCol = other;
        if (otherCol) otherCol.enabled = false;

        if (other.CompareTag("Crystal"))
        {
            if (_levelEnding) return;
            _levelEnding = true;

            // SFX
            if (crystalSfx) AudioManager.Instance?.PlaySfxWithVolume(crystalSfx, sfxVolume);

            // Hide visuals immediately; destroy later
            HideRenderers(other.gameObject);

            StartCoroutine(HandleLevelWinDeferred(other.gameObject));
            return;
        }

        if (other.CompareTag("Coin"))
        {
            // Guard GM/UI
            if (gameManager != null)
            {
                gameManager.coinCount++;
                coinCounterUI.GetComponent<ScalePop>().Pop();
                if (gameManager.counterUI != null)
                    gameManager.counterUI.CoinCounterUpdate(gameManager.coinCount);
            }

            if (pickupSfx) AudioManager.Instance?.PlaySfxWithVolume(pickupSfx, sfxVolume);

            HideRenderers(other.gameObject);
            StartCoroutine(DestroyEndOfFrame(other.gameObject));
            return;
        }

        if (other.CompareTag("Token"))
        {
            if (gameManager != null)
            {
                gameManager.tokenCount++;
                // Notify UI if assigned
                if (gameManager.Notification != null)
                    gameManager.Notification.PopNotification();
            }

            if (tokenSfx) AudioManager.Instance?.PlaySfxWithVolume(tokenSfx, sfxVolume);

            HideRenderers(other.gameObject);
            StartCoroutine(DestroyEndOfFrame(other.gameObject));
            return;
        }

        // If it wasn't one of our pickups, re-enable the collider we disabled
        if (otherCol) otherCol.enabled = true;
    }

    private System.Collections.IEnumerator HandleLevelWinDeferred(GameObject crystal)
    {
        yield return null;

        if (destroyCrystalOnPickup)
        {
            if (crystal) Destroy(crystal);
        }
        else
        {
            if (crystal) crystal.SetActive(false);
        }


        if (gameManager != null)
        {
            try { gameManager.GameWin(); }
            catch (System.Exception ex)
            {
            }
        }
    }

    private static System.Collections.IEnumerator DestroyEndOfFrame(GameObject go)
    {
        yield return null;
        if (go) Destroy(go);
    }

    private static void HideRenderers(GameObject go)
    {
        if (!go) return;
        var mrs = go.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var mr in mrs) mr.enabled = false;
        var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in srs) sr.enabled = false;
    }
}