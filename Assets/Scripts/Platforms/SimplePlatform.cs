using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class SimplePlatform : Platform, IDoubleTapble, ISwipeble, ITapble
{
    private SignalBus signalBus;
    private TapState tapState;
    private Renderer renderer;
    private TextPlatform priceView;
    [SerializeField] 
    private GameObject spikes;
    public int lenOfColors = ColorProvider.Colors.Count;
    public ResourceHolder resourceHolder;
    private readonly Color32 initialColor = new Color32(130, 131, 161, 255);
    public int WallAmount;
    private GameBalance gameBalance;
    private const float highOfWall = 0.4f;
    public void Construct(GameBalance gameBalance, SignalBus signalBus, TapState tapState, int indexOfLayer)
    {
        this.gameBalance = gameBalance;
        this.indexOfLayer = indexOfLayer;
        this.signalBus = signalBus;
        this.tapState = tapState;
    }
    public int Price
    {
        get => Convert.ToInt32(priceView.nameLabel.text);
        set => priceView.nameLabel.text = value.ToString();
    }
    private void Start()
    {
        layer = transform.parent.parent.gameObject;
        renderer = GetComponent<Renderer>();
        priceView = GetComponent<TextPlatform>();
        resourceHolder = FindObjectOfType<ResourceHolder>();
    }
    public void OnDoubleTap(Platform platformComponent)
    {
        if (tapState.State == TapState.TypeOfTap.Simple)
        {
            StartCoroutine(layer.GetComponent<PositionStabilizer>().UpdateXZ(platformComponent));
        }
    }
    public void OnSwipeStart()
    {
        StartCoroutine(layer.GetComponent<PositionStabilizer>().UpdateY());
    }
    public void OnSwipeEnd()
    {
        StopAllCoroutines();
        StartCoroutine(layer.GetComponent<PositionStabilizer>().StabilizeY());
    }
    public void OnTap()
    {
        switch (tapState.State)
        {
            case TapState.TypeOfTap.Wall:
                SetWallState();
                break;
            case TapState.TypeOfTap.Spike:
                SetSpikeState();
                break;
            case TapState.TypeOfTap.Return:
                ReturnEnemy();
                break;
        }
    }
    private void ReturnEnemy()
    {
        if (NotSuitableForReturn()) return;
        signalBus.Fire(new ReturnEnemySignal { platform = this });
        UpdatePrice();
    }
    private void SetSpikeState()
    {
        if (NotSuitableForSpike()) return;
        CreateSpikes();
        state = PlatformState.Type.Spike;
        DecreaseMoney();
        UpdatePrice();
    }
    private void SetWallState()
    {
        if (NotSuitableForWall()) return;
        CreateWall();
        state = PlatformState.Type.Wall;
        DecreaseMoney();
        UpdatePrice();
    }

    private void DecreaseMoney()
    {
        signalBus.Fire(new DecreaseMoneySignal { amount = Price });
    }
    
    private bool NotSuitableForSpike() => !resourceHolder.ValidateOperation(Price) || 
                                          enemies.Count > 0 || State == PlatformState.Type.Wall;
    private bool NotSuitableForWall() => !resourceHolder.ValidateOperation(Price) || 
                                         enemies.Count > 0 || State == PlatformState.Type.Spike;
    private bool NotSuitableForReturn() => !resourceHolder.ValidateOperation(Price) || enemies.Count < 0;

    public override void NextTurn()
    {
        DecreaseWallHp();
    }

    public override void ChangeState()
    {
        if (tapState.State == TapState.TypeOfTap.Simple) TurnOffPriceView();
        else TurnOnPriceView();
    }

    private void DecreaseWallHp()
    {
        if (State == PlatformState.Type.Wall)
        {
            WallAmount--;
            if (WallAmount < 1) DestroyWall();
            else renderer.material.color = ColorProvider.Colors[(WallAmount - 1) % lenOfColors];
        }
    }

    private async void DestroyWall()
    {
        state = PlatformState.Type.Simple;
        await Task.Delay(500);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.05f);
        renderer.material.color = initialColor;
    }

    private void CreateSpikes()
    {
        renderer.material.color = ColorProvider.Colors[SpikesAmount++ % lenOfColors];
        spikes.SetActive(true);
    }
    private void CreateWall()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, highOfWall);
        renderer.material.color = ColorProvider.Colors[WallAmount++ % lenOfColors];
    }

    private void TurnOffPriceView()
    {
        priceView.nameLabel.text = "";
    }

    private void TurnOnPriceView()
    {
        Price = gameBalance.GetPrice(this);
        priceView.UpdatePosition();
    }
    private void UpdatePrice()
    {
        Price = gameBalance.GetPrice(this);
    }
}
