using UnityEngine;
using Zenject;

public class TapState
{
    [Inject] 
    private SignalBus signalBus;
    public enum TypeOfTap
    {
        Wall,
        Simple,
        Spike,
        Return
    }

    public TypeOfTap State { get; private set; } = TypeOfTap.Simple;

    public void ChangeState(ChangeTapStateSignal signal)
    {
        State = signal.type;
    }
}
