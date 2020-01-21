using System;
using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using Zenject;

public class TouchDetector : MonoBehaviour {

  private GameObject platform;
  private SimplePlatformLayer simplePlatformLayer;
  [Inject] 
  private SignalBus signalBus;
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
      if (TapState.TypeOfTap.Simple != TapState.Instance.State) return;
      var doubleTap = platform?.GetComponent<IDoubleTapble>();
      doubleTap?.OnDoubleTap(platform.GetComponent<Platform>());
  }

  private void OnTap(Gesture gesture)
  {
      platform = gesture.pickedObject;
      if (platform == null)
      {
          SetSimpleState();
          return;
      }
      var tap = platform?.GetComponent<ITapble>();
      tap?.OnTap();
    
  }
  private void OnSwipeStart(Gesture gesture)
  {
      platform = gesture.pickedObject;
      SetSimpleState();
      var swipe = platform?.GetComponent<ISwipeble>();
      swipe?.OnSwipeStart();
  }
  private void OnSwipeEnd(Gesture gesture)
  { 
      var swipe = platform?.GetComponent<ISwipeble>();
      swipe?.OnSwipeEnd();
  }

  private void SetSimpleState()
  {
    signalBus.Fire(new ChangeTapStateSignal {type = TapState.TypeOfTap.Simple});
  }
}