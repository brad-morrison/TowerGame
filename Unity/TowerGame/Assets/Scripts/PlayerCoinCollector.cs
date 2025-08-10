using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Scoring")]
    public int coinScore = 0;

    [Header("Audio")]
    public AudioClip pickupSfx;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Detection (use any)")]
    public bool useTagCheck = true;
    public string coinTag = "Coin";
    public bool useLayerCheck = false;
    public LayerMask coinLayers; // set in Inspector if using layers

    [Header("Debug")]
    public bool logContacts = true;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Player collider should be non-trigger
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (logContacts) Debug.Log($"[Player] Trigger with {other.name} (tag='{other.tag}', layer={LayerMask.LayerToName(other.gameObject.layer)})", this);

        if (!IsCoin(other)) return;

        coinScore++;
        if (pickupSfx)
            AudioManager.Instance.PlaySfx(pickupSfx);



        Destroy(other.gameObject);
        if (logContacts) Debug.Log($"[Player] Collected coin. Total = {coinScore}", this);
    }

    bool IsCoin(Collider other)
    {
        bool tagOk = !useTagCheck || other.CompareTag(coinTag);
        bool layerOk = !useLayerCheck || ((coinLayers.value & (1 << other.gameObject.layer)) != 0);
        // If both checks are enabled, require BOTH; if only one enabled, use that; if none enabled, accept anything named with 'coin'.
        if (useTagCheck && useLayerCheck) return tagOk && layerOk;
        if (useTagCheck) return tagOk;
        if (useLayerCheck) return layerOk;

        // Fallback: name contains 'coin' (case-insensitive)
        return other.name.ToLower().Contains("coin");
    }
}