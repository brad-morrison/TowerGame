using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Scoring")]
    public GameManager gameManager;
    public UnityReact react;

    [Header("Audio")]
    public AudioClip pickupSfx;
    public AudioClip crystalSfx;
    public AudioClip tokenSfx;
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

            gameManager.coinCount++;
            gameManager.counterUI.CoinCounterUpdate(gameManager.coinCount);
            if (pickupSfx) AudioManager.Instance.PlaySfx(pickupSfx);
            Destroy(other.gameObject);
            
            //gameManager.counterUI.CoinCounterUpdate(coinScore);
        }
        
        // --- TOKEN ---
        if (lowerName.Contains("token"))
        {

            gameManager.tokenCount++;
            //gameManager.counterUI.CoinCounterUpdate(gameManager.coinCount);
            // fire notification
            gameManager.Notification.PopNotification();
            if (tokenSfx) AudioManager.Instance.PlaySfx(tokenSfx);
            Destroy(other.gameObject);
            
            //gameManager.counterUI.CoinCounterUpdate(coinScore);
        }
    }
}