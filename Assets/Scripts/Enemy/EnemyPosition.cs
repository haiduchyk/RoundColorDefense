﻿using ModestTree;
using Zenject;

public class EnemyPosition
{
    private static int indexOfFirstSimplePlatform = 3;
    [Inject] private EnemyController enemyController;
    
    private int IndexOfPlatformOnNextLayer(Enemy enemy)
    {
        var curPlatform = enemy.platform;

        var curPlatforms = GetPlatformsFromLayer(enemy.IndexOfLayer); 
        var nextPlatforms = GetPlatformsFromLayer(enemy.IndexOfLayer - 1); 
        
        enemyController.DisconnectEnemy(enemy);
        
        var curIndex = curPlatforms.IndexOf(curPlatform);
        
        var curLength = curPlatforms.Length;
        var nextLength = nextPlatforms.Length;
        
        var nextIndex = (int) (curIndex * ((double) nextLength / curLength));
        return nextIndex;
    }
     public Platform[] GetPrevPlatforms(Enemy enemy)
     {

         var nextIndexes = IndexesOfPlatformsOnPrevLayer(enemy);

         var nextPlatform1 = GetPlatformOnLayer(enemy.IndexOfLayer + 1, nextIndexes[0]);
         var nextPlatform2 = GetPlatformOnLayer(enemy.IndexOfLayer + 1, nextIndexes[1]);

         return new []{nextPlatform1, nextPlatform2};
 
     }
     private int[] IndexesOfPlatformsOnPrevLayer(Enemy enemy)
     {
         var curPlatform = enemy.platform;
        
         var curPlatforms = PlatformProvider.Instance.layers[enemy.IndexOfLayer].currentPlatforms;
         var prevPlatforms = PlatformProvider.Instance.layers[enemy.IndexOfLayer + 1].currentPlatforms;
        
         var curIndex = curPlatforms.IndexOf(curPlatform);
        
         var curLength = curPlatforms.Length;
         var prevLength = prevPlatforms.Length;
        
         var prevIndex = (int) (curIndex * ((double) prevLength / curLength));
         return enemy.IndexOfLayer == indexOfFirstSimplePlatform ? new[] {prevIndex, prevIndex} : new []{prevIndex, prevIndex + 1};
     }
     private int CurrentIndexOfEnemy(Enemy enemy) => PlatformProvider.Instance.layers[enemy.IndexOfLayer].currentPlatforms.IndexOf(enemy.platform);
    
     private Platform GetPlatformOnLayer(int numbOfLayer, int indexOfPlatform)
     {
         var nextPlatforms = PlatformProvider.Instance.layers[numbOfLayer].currentPlatforms;
         return nextPlatforms[indexOfPlatform];
     }
     
     public Platform GetNextPlatform(Enemy enemy)
     {
        
         var nextIndex = IndexOfPlatformOnNextLayer(enemy);
         
         var nextPlatform = GetPlatformOnLayer(enemy.IndexOfLayer - 1, nextIndex);
         if (nextPlatform.State != PlatformState.Type.Wall) return nextPlatform;
        
        
         var curIndex = CurrentIndexOfEnemy(enemy);

         var curPlatforms = GetPlatformsFromLayer(enemy.IndexOfLayer);
         var nextPlatforms = GetPlatformsFromLayer(enemy.IndexOfLayer - 1);
        
         var curLength = curPlatforms.Length;
         var nextLength = nextPlatforms.Length;

         var leftIsBlocked = false;
         var rightIsBlocked = false;

         var coof = (double)  nextLength / curLength;
        
         for (var i = 1; i < curLength - 1; i++)
         {
             var leftIndex = (curIndex - i + curLength) % curLength;
             if (!leftIsBlocked)
             {
                 if (curPlatforms[leftIndex].State != PlatformState.Type.Wall)
                 {
                     nextIndex = (int) (leftIndex * coof);
                     nextPlatform = nextPlatforms[nextIndex];
                     if (nextPlatform.State != PlatformState.Type.Wall) return curPlatforms[(curIndex - 1 + curLength) % curLength];
                 }
                 else leftIsBlocked = true;
             }
            
             var rightIndex = (curIndex + i + curLength) % curLength;
            
             if (!rightIsBlocked)
             {
                 if (curPlatforms[rightIndex].State != PlatformState.Type.Wall)
                 {
                     nextIndex = (int) (rightIndex * coof);
                     nextPlatform = nextPlatforms[nextIndex];
                     if (nextPlatform.State != PlatformState.Type.Wall) return curPlatforms[(curIndex + 1 + curLength) % curLength];
                 }
                 else rightIsBlocked = true;

             }

         }
        
         return curPlatforms[curIndex];
     }
    
     private Platform[] GetPlatformsFromLayer(int index) => PlatformProvider.Instance.layers[index].currentPlatforms;

}
