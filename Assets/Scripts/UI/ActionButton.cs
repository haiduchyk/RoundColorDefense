using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ActionButton : MonoBehaviour
{
    public Sprite on, off;
    Image spriteRender;
    private bool isActivated;
    private RectTransform rectTransform;
    [SerializeField]
    private TapState.TypeOfTap buttonState;
    private TapState.TypeOfTap CurType => !isActivated ? buttonState : TapState.TypeOfTap.Simple;
    [Inject]
    private SignalBus signalBus;

    private int size = 40;
    
    private void Start()
    {
        spriteRender = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void DisActivate()
    {
        if (isActivated)
        {
            isActivated = false;
            ChangeSprite();
        }
    }
    
    public void Activate()
    {
        isActivated = true;
        ChangeSprite();
    }
    
    public void OnTap()
    {
        ChangeState();
    }

    private void ChangeState()
    {
        signalBus.Fire(new ChangeTapStateSignal{type = CurType});        
    }

    private void ChangeSprite()
    {
        spriteRender.sprite = isActivated ? on : off;
        if (isActivated) IncreaseSize();
        else DecreaseSize();
    }

    private void IncreaseSize()
    {
        var rect = rectTransform.rect; 
        rectTransform.sizeDelta = new Vector2(rect.width + size, rect.height + size);
    }
    private void DecreaseSize()
    {
        var rect = rectTransform.rect; 
        rectTransform.sizeDelta = new Vector2(rect.width - size, rect.height - size);
    }
}
