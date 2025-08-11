using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerPickupCollector : MonoBehaviour
{
    [Header("Scoring")]
    public int coinScore = 0;
    public int crystalCount = 0;
    public GameManager gameManager;

    [Header("Audio")]
    public AudioClip coinSfx;
    public AudioClip crystalSfx;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Coin Detection (use any)")]
    public bool useCoinTagCheck = true;
    public string coinTag = "Coin";
    public bool useCoinLayerCheck = false;
    public LayerMask coinLayers;

    [Header("Crystal Detection (use any)")]
    public bool useCrystalTagCheck = true;
    public string crystalTag = "Crystal";
    public bool useCrystalLayerCheck = false;
    public LayerMask crystalLayers;

    [Header("Debug")]
    public bool logContacts = true;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;     // typical for trigger pickups
        rb.useGravity = false;

        // Player collider can be non-trigger if pickups are triggers.
        // If you prefer collisions, make sure the OTHER collider has a non-kinematic rigidbody.
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    // --- Triggers ---
    void OnTriggerEnter(Collider other) => HandleContact(other.gameObject);

    // --- Collisions (fallback if your crystal isn't a trigger) ---
    void OnCollisionEnter(Collision collision) => HandleContact(collision.collider.gameObject);

    void HandleContact(GameObject other)
    {
        if (logContacts)
        {
            Debug.Log($"[Player] Contact with {other.name} (tag='{other.tag}', layer={LayerMask.LayerToName(other.layer)})", this);
        }

        if (IsCoin(other))
        {
            coinScore++;
            if (gameManager) gameManager.coins = coinScore; 
            if (coinSfx) AudioManager.Instance.PlaySfx(coinSfx);
            Destroy(other);
            if (logContacts) Debug.Log($"[Player] Collected COIN. Total coins = {coinScore}", this);
            return;
        }

        if (IsCrystal(other))
        {
            crystalCount++;
            if (gameManager) gameManager.crystals = crystalCount; 
            if (crystalSfx) AudioManager.Instance.PlaySfx(crystalSfx);
            Destroy(other);
            if (logContacts) Debug.Log($"[Player] Collected CRYSTAL. Total crystals = {crystalCount}", this);
            return;
        }
    }

    bool IsCoin(GameObject go)
    {
        bool tagOk = !useCoinTagCheck || go.CompareTag(coinTag);
        bool layerOk = !useCoinLayerCheck || ((coinLayers.value & (1 << go.layer)) != 0);

        if (useCoinTagCheck && useCoinLayerCheck) return tagOk && layerOk;
        if (useCoinTagCheck) return tagOk;
        if (useCoinLayerCheck) return layerOk;

        return go.name.ToLower().Contains("coin");
    }

    bool IsCrystal(GameObject go)
    {
        bool tagOk = !useCrystalTagCheck || go.CompareTag(crystalTag);
        bool layerOk = !useCrystalLayerCheck || ((crystalLayers.value & (1 << go.layer)) != 0);

        if (useCrystalTagCheck && useCrystalLayerCheck) return tagOk && layerOk;
        if (useCrystalTagCheck) return tagOk;
        if (useCrystalLayerCheck) return layerOk;

        return go.name.ToLower().Contains("crystal");
    }
}
