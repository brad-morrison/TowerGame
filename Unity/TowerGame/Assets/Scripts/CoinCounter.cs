using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    public int coins { get; private set; }
    public GameManager gameManager;

    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
        gameManager.coins = coins;
        // No UI here—just a variable, as requested.
        Debug.Log($"Coins: {coins}");
    }
}