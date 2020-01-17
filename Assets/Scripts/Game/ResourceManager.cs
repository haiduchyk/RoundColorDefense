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
    const int initialAmountOfCoins = 30;
    [SerializeField]
    private TextMeshProUGUI scoreView;
    [SerializeField]
    private TextMeshProUGUI coinView; 
    [Inject] 
    private AudioManager audioManager;
    
    private Boolean resized;
    private Boolean IsResized
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
    
    public void DecreaseMoney(int amount)
    {
        Coins -= amount;
    }

    public bool ValidateOperation(int price)
    {
        return Coins >= price;
    }
    
    public int Score
    {
        get => Convert.ToInt32(scoreView.text);
        set => scoreView.text = value.ToString();
    }
  
    public int Coins
    {
        get => Convert.ToInt32(coinView.text);
        set => coinView.text = value.ToString();
    }
    
    void Start()
    {
        camera = Camera.main;
        Coins = initialAmountOfCoins;
    }


    private async Task WaitAll(List<Task> tasks)
    {
        foreach (var task in tasks)
        {
            await task;
        }
    }
    public async void GenerateCoins(Vector3 position, int amount)
    {
        Score += amount;
        var coins = new List<Coin>();
        var tasks = new List<Task>();
        for (int i = 0; i < amount; i++)
        {
            var enemyPosition = camera.WorldToScreenPoint(position);
            
            var random = Random.insideUnitCircle * 40;
            random += new Vector2(60 * random.x > 0 ? 1 : -1, 60 * random.y > 0 ? 1 : -1);
            var coinTransform = Instantiate(coinPrefab, canvas.transform).transform;
            coinTransform.position = enemyPosition;
            var coin = coinTransform.GetComponent<Coin>();
            var task = coin.AppearAnimation(coin.transform.position + new Vector3(random.x, random.y, 0));
            tasks.Add(task);
            coins.Add(coin);
        }

        await WaitAll(tasks);
        await Task.Delay(300);
        var delayBetweenCoins = 30;
        foreach (var coin in coins)
        {
            var moveAnimation = coin.GetComponent<Coin>().MoveAnimation(coinHandler.transform.position);
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
