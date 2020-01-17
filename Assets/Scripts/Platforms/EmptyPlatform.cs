using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EmptyPlatform : Platform
{
    void Start()
    {
        layer = transform.parent.gameObject;
    }
}
