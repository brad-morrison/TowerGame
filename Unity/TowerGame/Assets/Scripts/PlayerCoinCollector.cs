using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Scoring")]
    public int coinScore = 0;
    public GameManager gameManager;
    public UnityReact react;

    [Header("Audio")]
    public AudioClip pickupSfx;
    public AudioClip crystalSfx;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Detection (use any)")]
    public bool useTagCheck = true;
    public string coinTag = "Coin";
    public bool useLayerCheck = false;
    public LayerMask coinLayers;

    [Header("Crystal (win)")]
    public string crystalTag = "Crystal";
    public bool destroyCrystalOnPickup = true; // else we just disable it

    [Header("Debug")]
    public bool logContacts = true;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Player collider should be non-trigger; pickups should be triggers.
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (logContacts)
            Debug.Log($"[Player] Trigger with {other.name} (tag='{other.tag}', layer={LayerMask.LayerToName(other.gameObject.layer)})", this);

        // --- CRYSTAL: level complete ---
        if (IsCrystal(other))
        {
            if (crystalSfx) AudioManager.Instance.PlaySfx(crystalSfx);

            if (destroyCrystalOnPickup) Destroy(other.gameObject);
            else other.gameObject.SetActive(false);

            // Tell your GM you won (rename to your actual method)
            react.React_GameWonUI(true, gameManager.coins);

            if (logContacts) Debug.Log("[Player] CRYSTAL collected — level complete!", this);
            return; // stop here; no coin logic this frame
        }

        // --- COIN: normal pickup ---
        if (!IsCoin(other)) return;

        coinScore++;
        // if you track coins in GM, do it here:
        // if (gameManager) gameManager.SetCoins(coinScore);

        if (pickupSfx) AudioManager.Instance.PlaySfx(pickupSfx);
        Destroy(other.gameObject);

        if (logContacts) Debug.Log($"[Player] Collected coin. Total = {coinScore}", this);
    }

    bool IsCoin(Collider other)
    {
        bool tagOk   = !useTagCheck   || other.CompareTag(coinTag);
        bool layerOk = !useLayerCheck || ((coinLayers.value & (1 << other.gameObject.layer)) != 0);

        if (useTagCheck && useLayerCheck) return tagOk && layerOk;
        if (useTagCheck)   return tagOk;
        if (useLayerCheck) return layerOk;

        return other.name.ToLower().Contains("coin");
    }

    bool IsCrystal(Collider other)
    {
        // simplest: tag the crystal "Crystal"
        if (!string.IsNullOrEmpty(crystalTag) && other.CompareTag(crystalTag)) return true;

        // fallback: name contains "crystal"
        return other.name.ToLower().Contains("crystal");
    }
}
