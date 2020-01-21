using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class Field : MonoBehaviour
{
    private ILayer[] layers => PlatformProvider.Instance.layers;
    public void CreateField()
    { 
        GetLayers();
        CreateLayers();
    }
    
    private void CreateLayers()
    {
        foreach (var layer in layers)
        {
            layer.CreatePlatforms();
        }
    }

    public void NextTurn()
    {
        foreach (var layer in layers)
        {
            layer.NextTurn();
        }
    }
    public void ChangeState()
    {
        foreach (var layer in layers)
        {
            layer.ChangeState();
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

    public void Reset()
    {
        RebuildLayers();
    }

    private void RebuildLayers()
    {
        DestroyLayers();
        CreateLayers();
    }
    private void DestroyLayers()
    {
        foreach (var layer in layers)
        {
            layer.DestroyLayer();
        }
    }
}
