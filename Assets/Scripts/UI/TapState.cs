using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zenject;

public class TapState : MonoBehaviour
{
    public TypeOfTap State => state;
    private TypeOfTap state = TypeOfTap.Simple;

    [SerializeField]
    private ActionButton wallButton;
    [SerializeField]
    private ActionButton spikeButton;
    [SerializeField]
    private ActionButton returnButton;
    
    public enum TypeOfTap
    {
        Wall,
        Simple,
        Spike,
        Return
    }

    private void DisableAllButtons()
    {
        wallButton.DisActivate();
        spikeButton.DisActivate();
        returnButton.DisActivate();
    }
    
    private void SetWallState()
    {
        state = TypeOfTap.Wall;
        DisableAllButtons();
        wallButton.Activate();
        ShowPrices();
    }

    private void SetReturnState()
    {
        state = TypeOfTap.Return;
        DisableAllButtons();
        returnButton.Activate();
        ShowPrices();
    }
    
    private void SetSpikeState()
    {
        state = TypeOfTap.Spike;
        DisableAllButtons();
        spikeButton.Activate();
        ShowPrices();
    }
    
    private void SetSimpleState()
    {
        state = TypeOfTap.Simple;
        DisableAllButtons();
        HidePrices();
    }

    private void ShowPrices()
    {
        foreach (var layer in PlatformProvider.Instance.layers) 
            if (layer is SimplePlatformLayer l) l.TurnOnPrices();
    }
    private void HidePrices()
    {
        foreach (var layer in PlatformProvider.Instance.layers) 
            if (layer is SimplePlatformLayer l) l.TurnOffPrices();
    }

    public void ChangeState(TypeOfTap state)
    {
        switch (state)
        {
            case TypeOfTap.Simple:
                SetSimpleState();
                break;
            case TypeOfTap.Wall:
                SetWallState();
                break;
            case TypeOfTap.Spike:
                SetSpikeState();
                break;
            case TypeOfTap.Return:
                SetReturnState();
                break;
        }
    }
    
}
