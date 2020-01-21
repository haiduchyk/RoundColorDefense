using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CentrePlatform : Platform, ITapble
{
    [Inject]
    readonly SignalBus _signalBus;

    void Start()
    {
        layer = transform.parent.gameObject;
    }

    public void OnTap()
    {
        if (TapState.Instance.State == TapState.TypeOfTap.Simple) _signalBus.Fire<NextTurnSignal>();
    }
}
