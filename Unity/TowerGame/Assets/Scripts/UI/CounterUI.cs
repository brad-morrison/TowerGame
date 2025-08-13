using TMPro;
using UnityEngine;

public class CounterUI : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshPro CoinCounter;
    public TextMeshPro GameOver_CoinCount;
    public TextMeshPro GameOver_TokenCount;
    public TextMeshPro Win_CoinCount;
    public TextMeshPro Win_TokenCount;

    public void CoinCounterUpdate(int score)
    {
        CoinCounter.text = score.ToString();
    }

    public void SetGameOverUI()
    {
        GameOver_CoinCount.text = gameManager.coinCount.ToString();
        GameOver_TokenCount.text = gameManager.tokenCount.ToString();
    }

    public void SetWinUI()
    {
        Win_CoinCount.text = gameManager.coinCount.ToString();
        Win_TokenCount.text = gameManager.tokenCount.ToString();
    }
}
