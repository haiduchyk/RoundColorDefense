using HedgehogTeam.EasyTouch;
using UnityEngine;
using Zenject;

public class GameResetter : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;
    [SerializeField] 
    private HighScore highScore;
    [Inject] 
    private ResourceHolder resourceHolder;
    [Inject] 
    private SignalBus signalBus;
    
    public bool gameEnded;
    public void EndGame()
    {
        if (!gameEnded) return;
        EasyTouch.SetEnabled(true);
        endScreen.SetActive(true);
    }
    
    public void Restart()
    {
        signalBus.Fire<RestartGameSignal>();
        gameEnded = false;
        GameBalance.Reset();
        endScreen.SetActive(false);
    }
    
}
