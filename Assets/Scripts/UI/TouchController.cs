using System;
using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using Zenject;

public class TouchController : MonoBehaviour {

  private GameObject platform;
  private SimplePlatformLayer simplePlatformLayer;
  [Inject]
  private TapState tapState;

  [Inject] private AudioManager audioManager;
  private void Start () {
    EasyTouch.On_SwipeEnd += OnSwipeEnd;
    EasyTouch.On_SwipeStart += OnSwipeStart;
    EasyTouch.On_DoubleTap += OnDoubleTap;
    EasyTouch.On_SimpleTap += OnTap;
  }

  private void OnDoubleTap(Gesture gesture)
  {
    platform = gesture.pickedObject;
    if (TapState.TypeOfTap.Simple != tapState.State);
    var doubleTap = platform?.GetComponent<IDoubleTapble>();
    doubleTap?.OnDoubleTap(platform.GetComponent<Platform>());
  }
  
  private void OnTap(Gesture gesture)
  {
    platform = gesture.pickedObject;
    if (platform == null) tapState.ChangeState(TapState.TypeOfTap.Simple);
    var tap = platform?.GetComponent<ITapble>();
    tap?.OnTap();
    
  }
  private void OnSwipeStart(Gesture gesture)
  {
    platform = gesture.pickedObject;
    tapState.ChangeState(TapState.TypeOfTap.Simple);
    var swipe = platform?.GetComponent<ISwipeble>();
    swipe?.OnSwipeStart();
  }
  private void OnSwipeEnd(Gesture gesture)
  {
    var swipe = platform?.GetComponent<ISwipeble>();
    swipe?.OnSwipeEnd();
  }
}