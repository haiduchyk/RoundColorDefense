using UnityEngine;
using Zenject;

public class GameRoot : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    
    public void Start()
    {
        signalBus.Fire<InitGameSignal>();
    }
}
