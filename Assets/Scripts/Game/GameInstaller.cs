using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] 
    private Field field;
    [SerializeField] 
    private GameObject enemyPrefab;
    [SerializeField] 
    private ActionButtonView actionButtonView;
    [SerializeField] 
    private ResourceHolder resourceHolder;
    [SerializeField] 
    private AudioManager audioManager;
    [SerializeField] 
    private GameResetter gameResetter;
    [SerializeField] 
    private CoinGenerator coinGenerator;
    public override void InstallBindings()
    {
        
        Container.BindInstance(field);
        Container.BindInstance(resourceHolder);
        Container.BindInstance(audioManager);
        Container.BindInstance(gameResetter);
        Container.BindInstance(actionButtonView);
        Container.BindInstance(coinGenerator);
        
        Container.Bind<EnemyMover>().AsSingle();
        Container.Bind<EnemyPosition>().AsSingle();
        Container.Bind<EnemySpawner>().AsSingle().WithArguments(enemyPrefab);
        Container.Bind<GameBalance>().AsSingle();
        Container.Bind<TapState>().AsSingle();
        
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<NextTurnSignal>();
        Container.DeclareSignal<ReturnEnemySignal>();
        Container.DeclareSignal<ChangeTapStateSignal>();
        Container.DeclareSignal<EndGameSignal>();
        Container.DeclareSignal<RestartGameSignal>();
        Container.DeclareSignal<InitGameSignal>();
        Container.DeclareSignal<GenerateCoinsSignal>();
        Container.DeclareSignal<CoinReceivedSignal>();
        Container.DeclareSignal<DecreaseMoneySignal>();

        Container.BindSignal<NextTurnSignal>()
            .ToMethod<EnemyMover>(x => x.NextTurn).FromResolve();
        Container.BindSignal<NextTurnSignal>()
            .ToMethod<Field>(x => x.NextTurn).FromResolve();
        
        Container.BindSignal<ReturnEnemySignal>()
            .ToMethod<EnemyMover>(x => x.ReturnEnemy).FromResolve();
        
        Container.BindSignal<ChangeTapStateSignal>()
            .ToMethod<TapState>(x => x.ChangeState).FromResolve();
        Container.BindSignal<ChangeTapStateSignal>()
            .ToMethod<ActionButtonView>(x => x.ChangeState).FromResolve();
        Container.BindSignal<ChangeTapStateSignal>()
            .ToMethod<Field>(x => x.ChangeState).FromResolve();
        
        Container.BindSignal<RestartGameSignal>()
            .ToMethod<Field>(x => x.Reset).FromResolve();
        Container.BindSignal<RestartGameSignal>()
            .ToMethod<EnemyMover>(x => x.Reset).FromResolve();
        Container.BindSignal<RestartGameSignal>()
            .ToMethod<ResourceHolder>(x => x.Reset).FromResolve();
        Container.BindSignal<RestartGameSignal>()
            .ToMethod<GameBalance>(x => x.Reset).FromResolve();
        
        Container.BindSignal<InitGameSignal>()
            .ToMethod<Field>(x => x.CreateField).FromResolve();
        Container.BindSignal<InitGameSignal>()
            .ToMethod<EnemyMover>(x => x.Reset).FromResolve();
        
        Container.BindSignal<GenerateCoinsSignal>()
            .ToMethod<CoinGenerator>(x => x.GenerateCoins).FromResolve();

        Container.BindSignal<EndGameSignal>()
            .ToMethod<GameResetter>(x => x.EndGame).FromResolve();
        
        Container.BindSignal<DecreaseMoneySignal>()
            .ToMethod<ResourceHolder>(x => x.DecreaseMoney).FromResolve();
    }
}
