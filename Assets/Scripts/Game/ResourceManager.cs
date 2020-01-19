using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ResourceManager : MonoBehaviour
{ 
    public GameObject coinPrefab;
    public GameObject canvas;
    public Camera camera;
    public GameObject coinHandler;
    const int initialAmountOfCoins = 3;
    const int delayBetweenCoins = 30;
    private const int delayAfterCoinsAppearing = 300;
    [SerializeField]
    private TextMeshProUGUI scoreView;
    [SerializeField]
    private TextMeshProUGUI coinView; 
    [Inject] 
    private AudioManager audioManager;
    
    private bool resized;
    private bool IsResized
    {
        get => resized;
        set
        {
            coinHandler.transform.localScale *= value ? 1.3f : 1 / 1.3f; 
            resized = value;
        }
    }

    public void Reset()
    {
        Coins = initialAmountOfCoins;
        Score = 0;
    }
    
    public void DecreaseMoney(int amount) => Coins -= amount;
    public bool ValidateOperation(int price) => Coins >= price;

    public int Score
    {
        get => Convert.ToInt32(scoreView.text);
        set => scoreView.text = value.ToString();
    }

    private int Coins
    {
        get => Convert.ToInt32(coinView.text);
        set => coinView.text = value.ToString();
    }
    
    void Start()
    {
        camera = Camera.main;
        Coins = initialAmountOfCoins;
    }
    
    public async void GenerateCoins(Vector3 position, int amount)
    {
        UpdateScore(amount);
        var tasks = new List<Task>();
        var coins = new List<Coin>();

        for (int i = 0; i < amount; i++)
        {
            var shift = AreaWithHoleInside(100, 60); 
            var coinTransform = Instantiate(coinPrefab, canvas.transform).transform;
            coinTransform.position = camera.WorldToScreenPoint(position);
            var coin = coinTransform.GetComponent<Coin>();
            var task = coin.AppearAnimation(coin.transform.position + new Vector3(shift.x, shift.y, 0));
            tasks.Add(task);
            coins.Add(coin);
        }
        await Task.WhenAll(tasks);
        MakeMoveAnimation(coins);
    }

    private void UpdateScore(int amount) => Score += amount;


    private Vector2 AreaWithHoleInside(int areaRadius, int holeRadius)
    {
        var area = Random.insideUnitCircle * (areaRadius - holeRadius);
        area += new Vector2(holeRadius * area.x > 0 ? 1 : -1, holeRadius * area.y > 0 ? 1 : -1);
        return area;
    }
    
    private async Task MakeMoveAnimation(List<Coin> coins)
    {
        await Task.Delay(delayAfterCoinsAppearing);
        foreach (var coin in coins)
        {
            var moveAnimation = coin.MoveAnimation(coinHandler.transform.position);
            OnEnd(moveAnimation);
            await Task.Delay(delayBetweenCoins);
        }
    }
    

    private async Task OnEnd(Task moveAnimation)
    {
        await moveAnimation;
        if (IsResized)
        {
            StopAllCoroutines();
            IsResized = !IsResized;
        } 
        StartCoroutine(Resize());
        audioManager.Play("coin");
        Coins++;
    }

    private IEnumerator Resize()
    {
        IsResized = true;
        yield return Task.Delay(20);
        IsResized = false;
    }
}
