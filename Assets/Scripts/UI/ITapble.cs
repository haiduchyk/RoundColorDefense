using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITapble
{
    void OnTap();
}

public interface IDoubleTapble
{
    void OnDoubleTap(Platform platform);
}

public interface ISwipeble
{
    void OnSwipeStart();
    void OnSwipeEnd();

}
