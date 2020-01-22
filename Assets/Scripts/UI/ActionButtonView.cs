using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zenject;

public class ActionButtonView : MonoBehaviour
{
    [SerializeField]
    private ActionButton wallButton;
    [SerializeField]
    private ActionButton spikeButton;
    [SerializeField]
    private ActionButton returnButton;
    
    private void DisableAllButtons()
    {
        wallButton.DisActivate();
        spikeButton.DisActivate();
        returnButton.DisActivate();
    }
    
    private void SetWallState()
    {
        DisableAllButtons();
        wallButton.Activate();
    }

    private void SetReturnState()
    {
        DisableAllButtons();
        returnButton.Activate();
    }
    
    private void SetSpikeState()
    {
        DisableAllButtons();
        spikeButton.Activate();
    }
    
    private void SetSimpleState()
    {
        DisableAllButtons();
    }
    
    public void ChangeState(ChangeTapStateSignal signal)
    {
        switch (signal.type)
        {
            case TapState.TypeOfTap.Simple:
                SetSimpleState();
                break;
            case TapState.TypeOfTap.Wall:
                SetWallState();
                break;
            case TapState.TypeOfTap.Spike:
                SetSpikeState();
                break;
            case TapState.TypeOfTap.Return:
                SetReturnState();
                break;
        }
    }
    
}
