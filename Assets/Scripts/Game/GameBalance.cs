using System.Collections.Generic;
using System.Linq;
using Zenject;
using Random = UnityEngine.Random;

public class GameBalance
{

    private int turn;
    private int maxAmountOfEnemy = 12;
    [Inject] 
    private TapState tapState;

    public void Reset()
    {
        turn = 0;
    }
    
    public int[] NextEnemiesStats()
    { 
        turn++;
        var stats = GetStats();
        var distributeStats = Distribute(stats);
        return distributeStats;
    }

    private int[] Distribute(List<int> stats)
    {
        var res = new int[maxAmountOfEnemy];
        var indexes = Enumerable.Range(0, maxAmountOfEnemy - 1).ToList();
        var len = stats.Count;
        
        for (var i = 0; i < len; i++)
        {
            var el = stats[0];
            stats.RemoveAt(0);
            var index = indexes[Random.Range(0, indexes.Count - 1)];
            res[index] = el;
        }

        return res;
    }
    
    private List<int> GetStats()
    {
        var amount = Random.Range(1, maxAmountOfEnemy / 2) + Random.Range(1, maxAmountOfEnemy / 2);
        var list = new List<int>();

        for (var i = 0; i < amount; i++)
        {
            list.Add(Random.Range(amount, amount + 2 * turn) / amount);
        }
        return list;
    }
    
    public int GetPrice(SimplePlatform platform)
    {
        var price = -1;
        switch (tapState.State)
        {
            case TapState.TypeOfTap.Wall:
                price = PriceForWall(platform);
                break;
            case TapState.TypeOfTap.Spike:
                price = PriceForSpikes(platform);
                break;
            case TapState.TypeOfTap.Return:
                price = PriceForReturn(platform);
                break;
        }
        return price;
    }
    
    private int PriceForSpikes(SimplePlatform platform) => (4 - platform.indexOfLayer) + platform.SpikesAmount * 3 / 2;
    private int PriceForReturn(SimplePlatform platform) => (4 - platform.indexOfLayer) * turn / 2;
    private int PriceForWall(SimplePlatform platform) => (4 - platform.indexOfLayer) * turn / 5 + platform.WallAmount;
}
