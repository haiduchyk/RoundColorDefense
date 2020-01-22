using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CoinGenerator : MonoBehaviour
{
    [Inject] 
    private ResourceHolder resourceHolder;
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private GameObject canvas;
    private Camera camera;
    public GameObject coinHandler;
    private const int delayAfterCoinsAppearing = 300;
    const int delayBetweenCoins = 30;
    private bool resized;
    [Inject] 
    private SignalBus signalBus;
    private bool IsResized
    {
        get => resized;
        set
        {
            coinHandler.transform.localScale *= value ? 1.3f : 1 / 1.3f; 
            resized = value;
        }
    }

    void Start()
    {
        camera = Camera.main;
    }
    
    public async void GenerateCoins(GenerateCoinsSignal signal)
    {
        var amount = signal.amount;
        var position = signal.position;
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

    private void UpdateScore(int amount) => resourceHolder.Score += amount;


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
        signalBus.Fire<CoinReceivedSignal>();
        resourceHolder.Coins++;
    }

    private IEnumerator Resize()
    {
        IsResized = true;
        yield return Task.Delay(20);
        IsResized = false;
    }
}
