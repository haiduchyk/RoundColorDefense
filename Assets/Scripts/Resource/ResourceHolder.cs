using TMPro;
using UnityEngine;


public class ResourceHolder : MonoBehaviour
{
    private int coinsAmount;
    private int scoreAmount;
    const int initialAmountOfCoins = 3000;
    
    [SerializeField]
    private TextMeshProUGUI scoreView;
    [SerializeField]
    private TextMeshProUGUI coinView;
    [SerializeField]
    private TextMeshProUGUI highScoreView;

    private void Start()
    {
        Coins = initialAmountOfCoins;
    }

    public void Reset()
    {
        SaveScore();
        Coins = initialAmountOfCoins;
        Score = 0;
    }
    
    public void DecreaseMoney(int amount) => Coins -= amount;
    public bool ValidateOperation(int price) => Coins >= price;

    public int Score
    {
        get => scoreAmount;
        set
        {
            scoreAmount = value;
            scoreView.text = scoreAmount.ToString();
        }
    }

    public int Coins
    {
        get => coinsAmount;
        set
        {
            coinsAmount = value;
            coinView.text = coinsAmount.ToString();
        }
    }

    private void SaveScore()
    {
        if (Score > PlayerPrefs.GetInt("High Score", 0))
        {
            PlayerPrefs.SetInt("High Score", Score);
            highScoreView.text = "Best: " + Score;
        }
    }
}
