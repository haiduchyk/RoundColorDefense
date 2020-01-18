using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class SimplePlatform : Platform, IDoubleTapble, ISwipeble, ITapble
{
    public SignalBus signalBus;
    private TapState tapState;
    public override bool isTrap => SpikesAmount != 0;
    private Renderer renderer;
    private TextPlatform priceView;
    [SerializeField] 
    private GameObject spikes;
    public int lenOfColors = ColorProvider.Colors.Count;
    public ResourceManager resourceManager;
    private readonly Color32 initialColor = new Color32(130, 131, 161, 255);
    public int SpikesAmount;
    public int WallAmount;
    
    private const float highOfWall = 0.4f;
    
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
        resourceManager = FindObjectOfType<ResourceManager>();
        tapState = FindObjectOfType<TapState>();
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
        if (!resourceManager.ValidateOperation(Price))
        {
            return;
        }

        if (enemies.Any())
        {
            signalBus.Fire(new ReturnEnemySignal { platform = GetComponent<SimplePlatform>() });
            UpdatePrice();
        }

    }
    private void SetSpikeState()
    {
        var price = Convert.ToInt32(priceView.nameLabel.text);
        if (!resourceManager.ValidateOperation(price)) return;
        
        if (enemies.Count > 0) return;
        if (State == PlatformState.Type.Wall) return;
        CreateSpikes();
        state = PlatformState.Type.Spike;
        resourceManager.DecreaseMoney(Price);
        UpdatePrice();
    }
    private void SetWallState()
    {
        if (!resourceManager.ValidateOperation(Price))
        {
            return;
        }
        if (enemies.Count > 0) return;
        if (State == PlatformState.Type.Spike) return;
        CreateWall();
        state = PlatformState.Type.Wall;
        resourceManager.DecreaseMoney(Price);

        UpdatePrice();
    }

    public void NextTurn()
    {
        DecreaseWallHp();
    }
    
    private void DecreaseWallHp()
    {
        if (State == PlatformState.Type.Wall)
        {
            WallAmount--;
            if (WallAmount == 0) DestroyWall();
            else renderer.material.color = ColorProvider.Colors[WallAmount - 1 % lenOfColors];
        }
    }

    private async void DestroyWall()
    {
        state = PlatformState.Type.Simple;
        await Task.Delay(500);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.01f);
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
    public void TurnOffPriceView()
    {
        priceView.nameLabel.text = "";
    }
    public void TurnOnPriceView()
    {
        Price = GameBalance.GetPrice(this, tapState);
        priceView.UpdatePosition();
    }
    private void UpdatePrice()
    {
        Price = GameBalance.GetPrice(this, tapState);
    }
}
