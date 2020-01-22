using HedgehogTeam.EasyTouch;
using UnityEngine;
using Zenject;

public class GameResetter : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;
    [Inject] 
    private ResourceHolder resourceHolder;
    [Inject] 
    private SignalBus signalBus;
    
    public bool gameEnded;
    public void EndGame()
    {
        EasyTouch.SetEnabled(true);
        endScreen.SetActive(true);
    }
    
    public void Restart()
    {
        signalBus.Fire<RestartGameSignal>();
        endScreen.SetActive(false);
    }
    
}
