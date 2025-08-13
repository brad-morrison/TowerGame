using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Refs")]
    public GameManager gameManager;
    public UnityReact react; // beware: likely calls into JS

    [Header("Audio")]
    public AudioClip pickupSfx;
    public AudioClip crystalSfx;
    public AudioClip tokenSfx;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Crystal (win)")]
    public bool destroyCrystalOnPickup = true;

    [Header("Debug")]
    public bool logContacts = true;

    // one-shot guards
    private bool _levelEnding = false;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = GetComponent<Collider>();
        col.isTrigger = false; // player solid; pickups must be triggers
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other) return;

        // Cheap filter: only react to triggers
        if (!other.isTrigger) return;

        // Stop duplicates ASAP
        var otherCol = other;
        if (otherCol) otherCol.enabled = false;

        // Route by tag (set these on your pickup prefabs)
        if (other.CompareTag("Crystal"))
        {
            if (_levelEnding) return;
            _levelEnding = true;

            // SFX
            if (crystalSfx) AudioManager.Instance?.PlaySfxWithVolume(crystalSfx, sfxVolume);

            // Hide visuals immediately; destroy later
            HideRenderers(other.gameObject);

            if (logContacts) Debug.Log("[Player] CRYSTAL collected — level complete!", this);

            // Defer heavy stuff one frame to avoid use-after-destroy/physics reentry
            StartCoroutine(HandleLevelWinDeferred(other.gameObject));
            return;
        }

        if (other.CompareTag("Coin"))
        {
            // Guard GM/UI
            if (gameManager != null)
            {
                gameManager.coinCount++;
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
        // Minor delay lets physics settle and avoids calling into UI/JS during trigger dispatch
        yield return null;

        if (destroyCrystalOnPickup)
        {
            if (crystal) Destroy(crystal);
        }
        else
        {
            if (crystal) crystal.SetActive(false);
        }

        // React bridge (guarded + try/catch)
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (react && gameManager != null)
            {
                react.React_GameWonUI(true, gameManager.coins);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"React bridge failed (ignored): {ex}");
        }
#endif

        // End level (make sure GameWin is idempotent)
        if (gameManager != null)
        {
            try { gameManager.GameWin(); }
            catch (System.Exception ex)
            {
                Debug.LogError($"GameWin threw: {ex}");
            }
        }
        else
        {
            Debug.LogWarning("GameManager not assigned on PlayerCoinCollector.");
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
        // Optional: also mute particle systems here
    }
}