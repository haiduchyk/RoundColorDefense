using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public Field field;
    public GameObject enemyPrefab;
    public TapState tapState;
    public ResourceManager resourceManager;
    public AudioManager audioManager;
    public GameController gameController;
    public override void InstallBindings()
    {
        
        Container.BindInstance(field).AsSingle();
        Container.BindInstance(resourceManager);
        Container.BindInstance(tapState);
        Container.BindInstance(audioManager);
        Container.BindInstance(gameController);
        
        Container.Bind<EnemyController>().AsSingle();
        Container.Bind<EnemyPosition>().AsSingle();
        Container.Bind<EnemySpawner>().AsSingle().WithArguments(enemyPrefab);

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<NextTurnSignal>();
        Container.DeclareSignal<ReturnEnemySignal>();
        
        Container.BindSignal<NextTurnSignal>()
            .ToMethod<EnemyController>(x => x.NextTurn).FromResolve();
        Container.BindSignal<ReturnEnemySignal>()
            .ToMethod<EnemyController>(x => x.ReturnEnemy).FromResolve();
        Container.BindSignal<NextTurnSignal>()
            .ToMethod<Field>(x => x.NextTurn).FromResolve();
    }
}
