using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner
{
    private readonly GameObject enemyPrefab;
    private const int indexOfEmptyLayer = 5;
    private readonly Vector3 newEnemiesSpawnPoint = new Vector3(0, 15f, 0f);
    [Inject]
    private EnemyMover enemyMover;

    public EnemySpawner(GameObject enemyPrefab)
    {
        this.enemyPrefab = enemyPrefab;
    }
    public List<Enemy> CreateEnemies()
    {
        var enemiesStats = GameBalance.NextEnemiesStats();
        var emptyPlatforms = PlatformProvider.Instance.layers[indexOfEmptyLayer].currentPlatforms;
        var enemies = new List<Enemy>();
        for (var i = 0; i < enemiesStats.Length; i++)
        {
            var hp = enemiesStats[i];
            if (hp == 0) continue;
            var enemy = GenerateEnemy(emptyPlatforms[i], hp);
            enemies.Add(enemy);
        }
        return enemies;
    }
    public Enemy GenerateEnemy(Platform emptyPlatform, int hp = 1)
    {
        var position = emptyPlatform.indexOfLayer == indexOfEmptyLayer
            ? newEnemiesSpawnPoint
            : new Vector3(0, 0, emptyPlatform.layer.GetComponent<SimplePlatformLayer>().distanceBetweenLayer);
                
        var enemyObject = Object.Instantiate(
            original: enemyPrefab,
            position,
            Quaternion.Euler(0, 0, 0)
        );
        enemyObject.transform.RotateAround(Vector3.zero, Vector3.up, (360 - emptyPlatform.CurrentAngle) + 90);

        var enemy = enemyObject.GetComponent<Enemy>();
        enemy.futureHp = hp;
        enemy.Hp = hp;
        enemyMover.ConnectEnemy(enemy, emptyPlatform);
        return enemy;
    }
}
