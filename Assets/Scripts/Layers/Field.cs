using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class Field : MonoBehaviour
{
    public void CreateField()
    { 
        GetLayers();
        CreateLayers();
    }
    
    private void CreateLayers()
    {
        var layers = PlatformProvider.Instance.layers;
        foreach (var layer in layers)
        {
            layer.CreatePlatforms();
        }
    }

    public void NextTurn()
    {
        var layers = PlatformProvider.Instance.layers;
        foreach (var layer in layers)
        {
            layer.NextTurn();
        }
    }
    private void GetLayers()
    {
        var layers = new ILayer[transform.childCount];
        for (var i = 0; i < transform.childCount; i++) {
            layers[i] = transform.GetChild(i).GetComponent<ILayer>();
        }
        PlatformProvider.Instance.layers = layers;
    }
    public void DestroyLayers()
    {
        var layers = PlatformProvider.Instance.layers;
        foreach (var layer in layers)
        {
            layer.DestroyLayer();
        }
    }
}
