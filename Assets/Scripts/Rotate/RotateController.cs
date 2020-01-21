using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using UnityEngine.Networking;

public interface IRotateController
{
     void ReshuffleAfterYRotation(int firstY, int secondY, int angleBetweenPlatforms);
     void ReshuffleAfterXZRotation(Platform platform);
}

public class SimpleRotateController : IRotateController
{
    private SimplePlatformLayer SimplePlatformLayer { get;}

    public SimpleRotateController(SimplePlatformLayer simplePlatformLayer)
    { 
        SimplePlatformLayer = simplePlatformLayer;
    }

    private int CalculateShift(int firstY, int secondY, int angleBetweenPlatforms)
    {
        var shift = ((firstY - secondY)) / angleBetweenPlatforms;
        return shift;
    }

    public void ReshuffleAfterYRotation(int firstY, int secondY, int angleBetweenPlatforms)
    {
        var shift = CalculateShift(firstY, secondY, angleBetweenPlatforms);
        var oldFront = SimplePlatformLayer.frontPlatforms;
        var oldBack = SimplePlatformLayer.backPlatforms;
        var newFront = YRearrange(oldFront, shift);
        var newBack = YRearrange(oldBack, shift);
        SimplePlatformLayer.frontPlatforms = newFront;
        SimplePlatformLayer.backPlatforms = newBack;
    }


    private Platform[] YRearrange(Platform[] platforms, int shift)
    {
        var len = platforms.Length;
        var copy = new Platform[len];
        for (var i = 0; i < len; i++)
        {
            var newPosition = (i + shift + len) % len;
            copy[newPosition] = platforms[i];
        }
        return copy;
    }

    public void ReshuffleAfterXZRotation(Platform platform)
    {
        var oldFront = SimplePlatformLayer.frontPlatforms;
        var oldBack = SimplePlatformLayer.backPlatforms;
        var index = SimplePlatformLayer.currentPlatforms.IndexOf(platform);

        var newFront = XZRearrange(oldFront, index);
        var newBack = XZRearrange(oldBack, index);
        SimplePlatformLayer.frontPlatforms = newFront;
        SimplePlatformLayer.backPlatforms = newBack;
    }

    private Platform[] XZRearrange(Platform[] platforms, int index)
    {
        var len = platforms.Length;
        var copy = new Platform[len];
        var shift = len / 2;
        for (var i = 0; i < len / 2; i++)
        {
            
            copy[(i + index + len) % len] = platforms[(i + index + shift + len) % len];
            copy[(i + index + shift + len) % len] = platforms[(i + index + len) % len];
            copy[(i + index + len / 2) % len] = platforms[(i + index + shift + len / 2) % len];
            copy[(i + index + shift + len / 2) % len] = platforms[(i + index + len / 2) % len];
            shift -= 2;
        }

        return copy;
    }
}

public class RotateControllerForSecondLayer : IRotateController
{
    public SimplePlatformLayer SimplePlatformLayer { get; set; }
    public RotateControllerForSecondLayer(SimplePlatformLayer simplePlatformLayer)
    {
        this.SimplePlatformLayer = simplePlatformLayer;
    }

    private int CalculateShift(int firstY, int secondY, int angleBetweenPlatforms)
    {
        var shift = ((firstY - secondY)) * 2 / angleBetweenPlatforms;
        return shift;
    }

    public void ReshuffleAfterYRotation(int firstY, int secondY, int angleBetweenPlatforms)
    {
        var shift = CalculateShift(firstY, secondY, angleBetweenPlatforms);
        var oldFront = SimplePlatformLayer.frontPlatforms;
        var oldBack = SimplePlatformLayer.backPlatforms;
        var newFront = YRearrange(oldFront, shift);
        var newBack = YRearrange(oldBack, shift);
        SimplePlatformLayer.frontPlatforms = newFront;
        SimplePlatformLayer.backPlatforms = newBack;
    }


    private Platform[] YRearrange(Platform[] platforms, int shift)
    {
        var len = platforms.Length;
        var copy = new Platform[len];

        for (var i = 0; i < len; i++)
        {
            copy[(i + shift + len ) % len] = platforms[i];
        }
        return copy;
    }
    
    public void ReshuffleAfterXZRotation(Platform platform)
    {
        var oldFront = SimplePlatformLayer.frontPlatforms;
        var oldBack = SimplePlatformLayer.backPlatforms;
        var index1 = -1;
        var index2 = -1;
        GetIndexesOfPlatform(ref index1, ref index2, platform);
        var newFront = XZRearrange(oldFront, index1, index2);
        var newBack = XZRearrange(oldBack, index1, index2);
        SimplePlatformLayer.frontPlatforms = newFront;
        SimplePlatformLayer.backPlatforms = newBack;
    }
    private void GetIndexesOfPlatform(ref int index1, ref int index2, Platform platform)
    {
        for (var i = 0; i < SimplePlatformLayer.currentPlatforms.Length; i++)
        {
            if (SimplePlatformLayer.currentPlatforms[i] == platform)
            {
                if (index1 == -1) index1 = i;
                else index2 = i;
            }
        }
    }
    private Platform[] XZRearrange(Platform[] platforms, int index1, int index2)
    {
        var len = platforms.Length;
        var copy = (Platform[]) platforms.Clone();
        var shift = 2;
        if (index2 == len - 1 && index1 == 0) shift *= -1;
        
        copy[(index1 - shift + len) % len] = platforms[index1];
        copy[index1] = platforms[(index1 - shift + len) % len];
        
        copy[(index2 + shift + len) % len] = platforms[index2];
        copy[index2] = platforms[(index2 + shift + len) % len];
        return copy;
    }
}