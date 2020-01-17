using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class GameBalance
{

    private static int turn;
    private static int maxAmount = 12;

    public static void Reset()
    {
        turn = 0;
    }
    
    public static int[] NextEnemiesStats()
    { 
        turn++;
        var stats = GetStats();
        var distributeStats = Distribute(stats);
        return distributeStats;
    }

    private static int[] Distribute(List<int> stats)
    {
        var res = new int[maxAmount];
        var indexes = Enumerable.Range(0, maxAmount - 1).ToList();
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
    
    private static List<int> GetStats()
    {
        var amount = Random.Range(1, maxAmount / 2) + Random.Range(1, maxAmount / 2);
        var list = new List<int>();

        for (var i = 0; i < amount; i++)
        {
            list.Add(Random.Range(amount, amount + 2 * turn) / amount);
        }
        return list;
    }
    
    public static int GetPrice(SimplePlatform platform, TapState tapState)
    {
        var price = 0;
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
    
    private static int PriceForSpikes(SimplePlatform platform) => (4 - platform.indexOfLayer) + platform.SpikesAmount;
    private static int PriceForReturn(SimplePlatform platform) => (4 - platform.indexOfLayer) * turn;
    private static int PriceForWall(SimplePlatform platform) => (4 - platform.indexOfLayer) * 2 + platform.WallAmount;


}
