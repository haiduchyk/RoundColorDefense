using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HedgehogTeam.EasyTouch;
using ModestTree;
using Zenject;
using Object = UnityEngine.Object;

public class EnemyMover
{
    const float speedForSimpleMove = 10f;
    const float speedForAppearingMove = 17f;
    const int amountOfLayers = 6;
    private readonly List<Task> tasks = new List<Task>();
    private List<Enemy>[] allEnemies = new List<Enemy>[amountOfLayers];
    private bool gameEnded;
    [Inject]
    private EnemySpawner enemySpawner;
    [Inject]
    private EnemyPosition enemyPosition;
    [Inject]
    private readonly SignalBus signalBus;
    
    private void CreateEnemiesList()
    { 
        allEnemies = new List<Enemy>[amountOfLayers];

        for (var i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i] = new List<Enemy>();
        }
    }
    
    private bool CanMove(Enemy enemy) =>
        PlatformProvider.Instance.layers[enemy.IndexOfLayer].currentPlatforms.IndexOf(enemy.platform) != -1;

    private void UpdateHp()
    {
        foreach (var enemyLayer in allEnemies)
        {
            foreach (var enemy in enemyLayer)
            {
                enemy.Hp = enemy.futureHp;
            }
        }
    }

    private List<Enemy> CreateEnemies() => enemySpawner.CreateEnemies();

    private bool LocateReturnedEnemy(Enemy enemy)
    {
        var prevPlatforms = enemyPosition.GetPrevPlatforms(enemy);

        var prevPlatform1 = prevPlatforms[0];
        var prevPlatform2 = prevPlatforms[1];

        var prev1IsWall = prevPlatform1.State == PlatformState.Type.Wall;
        var prev2IsWall = prevPlatform2.State == PlatformState.Type.Wall;

        if (prevPlatform1 == prevPlatform2)
        {
            DisconnectEnemy(enemy);
            ConnectEnemy(enemy, prevPlatform1);
        }
        else if (prev1IsWall && prev2IsWall) return false;
        else if (prev1IsWall || prev2IsWall)
        {
            DisconnectEnemy(enemy);
            ConnectEnemy(enemy, prev1IsWall ? prevPlatform2 : prevPlatform1);
        }
        else DivideEnemy(enemy, prevPlatform1, prevPlatform2);
        

        return true;
    }

    private void DivideEnemy(Enemy enemy, Platform prevPlatform1, Platform prevPlatform2)
    {
        var hp = enemy.Hp / 2;
        if (hp == 0)
        {
            DisconnectEnemy(enemy);
            ConnectEnemy(enemy, prevPlatform1);
            return;
        } 
            
        var divideEnemy = enemySpawner.GenerateEnemy(enemy.platform, hp);
        divideEnemy.transform.position = enemy.transform.position;
            
        enemy.Hp = hp;
        enemy.futureHp = hp;

        DisconnectEnemy(enemy);
        DisconnectEnemy(divideEnemy);
            
        ConnectEnemy(enemy, prevPlatform1);
        ConnectEnemy(divideEnemy, prevPlatform2);
        
        MakeStepAnimation(new List<Enemy>() {divideEnemy}, speedForSimpleMove);
        CheckForTrigger(new List<Enemy>() {divideEnemy});
    }

    public async void ReturnEnemy(ReturnEnemySignal signal)
    { 
        EasyTouch.SetEnabled(false);
        var enemy = signal.platform.enemies[0];
        var success = LocateReturnedEnemy(enemy);
        
        if (!success) signalBus.Fire<CancelOperationSignal>();
        else
        {
            signalBus.Fire(new DecreaseMoneySignal { amount = signal.platform.Price });
            MakeStepAnimation(new List<Enemy>() {enemy}, speedForSimpleMove);
            CheckForTrigger(new List<Enemy>() {enemy});
            await AfterMovementOperations();
        }
        EasyTouch.SetEnabled(true);
    }

    public async void NextTurn()
    {
        EasyTouch.SetEnabled(false);
        await MoveExistEnemies();
        await AfterMovementOperations();
        if (gameEnded) return;
        
        var newEnemies = CreateEnemies();
        await MoveNewEnemies(newEnemies);
        await AfterMovementOperations();
        
        EasyTouch.SetEnabled(true);
    }

    private async Task AfterMovementOperations()
    {
        await Task.WhenAll(tasks);
        ClearDeadEnemies();
        UpdateHp();
        if (gameEnded) signalBus.Fire<EndGameSignal>();
    }


    private async Task MoveExistEnemies()
    {
        foreach (var layerEnemies in allEnemies)
        {
            var movableEnemies = layerEnemies.FindAll(CanMove);
            await MoveEnemies(movableEnemies, speedForSimpleMove);

        }
    }
    
    private async Task MoveNewEnemies(List<Enemy> newEnemies)
    {
        await MoveEnemies(newEnemies, speedForAppearingMove); 
    }

    private async Task MoveEnemies(List<Enemy> enemies, float speed)
    {
        SetTargets(enemies);
        MakeStepAnimation(enemies, speed);
        CheckForTrigger(enemies);
    }

    private void ClearDeadEnemies()
    {
        foreach (var layerEnemies in allEnemies)
        {
            layerEnemies.RemoveAll(enemy =>
            {
                if (enemy.isDead)
                {
                    DestroyEnemy(enemy);
                    return true;
                }
                return false;
            });
        }
    }
    
    private void MakeStepAnimation(List<Enemy> movableEnemies, float speed)
    {
        foreach (var enemy in movableEnemies)
        {
            tasks.Add(enemy.UpdatePosition(speed));
        }
    }

    private void SetTargets(List<Enemy> movableEnemies)
    {
        foreach (var enemy in movableEnemies)
        {
            if (CanMove(enemy)) SetNextPosition(enemy);
        }
    }
    private void SetNextPosition(Enemy enemy)
    {
        var nextPlatform = GetNextPlatform(enemy);
        ConnectEnemy(enemy, nextPlatform);
    }
    private Platform GetNextPlatform(Enemy enemy)
    {
        var nextPlatform = enemyPosition.GetNextPlatform(enemy);
        return nextPlatform;
    }

    private void CheckForTrigger(List<Enemy> movableEnemies)
    {
        CheckForTrap(movableEnemies);
        CheckMoreThanOne(movableEnemies);
    }

    private void CheckMoreThanOne(List<Enemy> movableEnemies)
    {
        foreach (var enemy in movableEnemies)
        {
            var platform = enemy.platform;
            if (AmountOfAliveEnemy(platform.enemies) > 1) MergeEnemies(platform.enemies);
        }
    }
    private void CheckForTrap(List<Enemy> movableEnemies)
    {
        foreach (var enemy in movableEnemies)
        {        
            var platform = enemy.platform;
            if (platform.isTrap) DecreaseEnemyHp(enemy);
            if (enemy.IndexOfLayer == 0) EndGame(enemy);
        }
    }
    private int AmountOfAliveEnemy(List<Enemy> enemies) => enemies.FindAll(enemy => !enemy.isDead).Count;
    private void DecreaseEnemyHp(Enemy enemy)
    {
        var platform = enemy.platform;
        var losedHp = enemy.futureHp - platform.SpikesAmount >= 0 ? platform.SpikesAmount : enemy.futureHp;
        signalBus.Fire(new GenerateCoinsSignal(losedHp, enemy.Target));
        enemy.futureHp -= platform.SpikesAmount;
        if (enemy.futureHp < 1) KillEnemy(enemy);
    }
    
    public void Reset()
    {
        CreateEnemiesList();
        gameEnded = false;
    }

    private void MergeEnemies(List<Enemy> enemies)
    {
        var mergedEnemy = enemies[0];
        for (var i = 1; i < enemies.Count; i++)
        {
            mergedEnemy.futureHp += enemies[i].futureHp;
            enemies[i].isDead = true;
        }
    }

    private void KillEnemy(Enemy enemy)
    {
        enemy.isDead = true;
    }

    private void DestroyEnemy(Enemy enemy)
    {
        enemy.platform.enemies.Remove(enemy);
        Object.Destroy(enemy.transform.gameObject);
    }

    private void EndGame(Enemy enemy)
    {
        KillEnemy(enemy);
        gameEnded = true;
    }

    public void DisconnectEnemy(Enemy enemy)
    {
        allEnemies[enemy.IndexOfLayer].Remove(enemy);
        enemy.platform.enemies.Remove(enemy);
    }
    
    public void ConnectEnemy(Enemy enemy, Platform platform)
    {
        enemy.platform = platform;
        platform.enemies.Add(enemy);
        enemy.transform.SetParent(platform.transform);
        allEnemies[enemy.IndexOfLayer].Add(enemy);
    }
}
