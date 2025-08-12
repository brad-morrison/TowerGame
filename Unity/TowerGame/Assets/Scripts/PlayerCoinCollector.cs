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

    [Header("Crystal (win)")]
    public bool destroyCrystalOnPickup = true;

    [Header("Debug")]
    public bool logContacts = true;

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = GetComponent<Collider>();
        col.isTrigger = false; // Player is solid, pickups are triggers
    }

    void OnTriggerEnter(Collider other)
    {
        if (logContacts)
            Debug.Log($"[Player] Trigger with {other.name}", this);

        string lowerName = other.name.ToLower();

        // --- CRYSTAL ---
        if (lowerName.Contains("crystal"))
        {
            if (crystalSfx) AudioManager.Instance.PlaySfx(crystalSfx);

            if (destroyCrystalOnPickup) Destroy(other.gameObject);
            else other.gameObject.SetActive(false);

            if (react) react.React_GameWonUI(true, gameManager.coins);

            if (logContacts) Debug.Log("[Player] CRYSTAL collected — level complete!", this);
            
            gameManager.GameWin();
            return;
        }

        // --- COIN ---
        if (lowerName.Contains("coin"))
        {
            coinScore++;
            if (pickupSfx) AudioManager.Instance.PlaySfx(pickupSfx);
            Destroy(other.gameObject);

            if (logContacts) Debug.Log($"[Player] Collected coin. Total = {coinScore}", this);
        }
    }
}