﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HedgehogTeam.EasyTouch;
using ModestTree;
using Zenject;
using Object = UnityEngine.Object;

public class EnemyController
{
    const float speedForSimpleMove = 10f;
    const float speedForAppearing = 17f;
    const int amountOfLayers = 6;
    private readonly List<Task> tasks = new List<Task>();
    private List<Enemy>[] allEnemies = new List<Enemy>[amountOfLayers];
    
    [Inject]
    private EnemySpawner enemySpawner;
    [Inject]
    private EnemyPosition enemyPosition;
    [Inject]
    readonly SignalBus _signalBus;
    [Inject]
    private ResourceManager resourceManager;
    [Inject]
    private AudioManager audioManager;

    [Inject] private GameController gameController;
    
    public void CreateEmptyEnemies()
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
        var curPlatform = enemy.platform;
        
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
        else if (prev1IsWall && prev2IsWall)
        {
            return false;
        }
        else if (prev1IsWall || prev2IsWall)
        {
            DisconnectEnemy(enemy);
            ConnectEnemy(enemy, prev1IsWall ? prevPlatform2 : prevPlatform1);
        }
        else
        {
            var hp = enemy.Hp / 2;
            if (hp == 0)
            {
                DisconnectEnemy(enemy);
                ConnectEnemy(enemy, prevPlatform1);
            } 
            
            var divideEnemy = enemySpawner.GenerateEnemy(curPlatform, hp);
            divideEnemy.transform.position = curPlatform.transform.position;
            
            enemy.Hp = hp;
            enemy.futureHp = hp;

            DisconnectEnemy(enemy);
            DisconnectEnemy(divideEnemy);
            

            ConnectEnemy(enemy, prevPlatform1);
            ConnectEnemy(divideEnemy, prevPlatform2);

            
            MakeStepAnimation(new List<Enemy>() {divideEnemy}, speedForSimpleMove);
            CheckForTrigger(new List<Enemy>() {divideEnemy});
        }

        return true;
    }
    

    public async void ReturnEnemy(ReturnEnemySignal signal)
    {
        if (!signal.platform.enemies.Any())
        {
            audioManager.Play("cancel");
            return;
        };
        var enemy = signal.platform.enemies[0];
        
        EasyTouch.SetEnabled(false);

        var success = LocateReturnedEnemy(enemy);

        if (!success)
        {
            audioManager.Play("cancel");
        }
        else
        {
            resourceManager.DecreaseMoney(signal.platform.Price);
            MakeStepAnimation(new List<Enemy>() {enemy}, speedForSimpleMove);
            CheckForTrigger(new List<Enemy>() {enemy});
            await WaitAll();
            UpdateHp();
            ClearEnemies();
        }

        EasyTouch.SetEnabled(true);
    }


    public async void NextTurn()
    {
        EasyTouch.SetEnabled(false);
        await MoveExistEnemies();
        var success = await AfterMovementOperations();
        if (!success) return;
        
        var newEnemies = CreateEnemies();
        await MoveNewEnemies(newEnemies);
        await AfterMovementOperations();
        
        EasyTouch.SetEnabled(true);
    }

    private async Task<bool> AfterMovementOperations()
    {
        await WaitAll();
        ClearEnemies();
        UpdateHp();
        if (gameController.gameEnded)
        {
            gameController.EndGame();
            return false;
        }
        return true;
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
        await MoveEnemies(newEnemies, speedForAppearing); 
    }

    private async Task MoveEnemies(List<Enemy> enemies, float speed)
    {
        SetTargets(enemies);
        MakeStepAnimation(enemies, speed);
        CheckForTrigger(enemies);
    }

    private void ClearEnemies()
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

    private async Task WaitAll()
    {
        foreach (var task in tasks)
        {
            await task;
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
        var nextPlatform = enemyPosition.CalculateIndexOfNextPlatform(enemy);
        return nextPlatform;
    }

    private void CheckForTrigger(List<Enemy> movableEnemies)
    {
        foreach (var enemy in movableEnemies)
        {        
            var platform = enemy.platform;
            if (platform.isTrap) DecreaseEnemyHp(enemy);
            if (enemy.IndexOfLayer == 0) EndGame(enemy);
        }

        foreach (var enemy in movableEnemies)
        {
            var platform = enemy.platform;
            if (AmountOfAliveEnemy(platform.enemies) > 1) MergeEnemies(platform.enemies);
        }
    }
    private int AmountOfAliveEnemy(List<Enemy> enemies) => enemies.FindAll(enemy => !enemy.isDead).Count;
    private void DecreaseEnemyHp(Enemy enemy)
    {
        var platform = (SimplePlatform) enemy.platform;
        
        var losedHp = enemy.futureHp - platform.SpikesAmount >= 0 ? platform.SpikesAmount : enemy.futureHp;
        
        resourceManager.GenerateCoins(enemy.Target, losedHp);

        enemy.futureHp -= platform.SpikesAmount;
        
        if (enemy.futureHp < 1) KillEnemy(enemy);
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
        gameController.gameEnded = true;
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