using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformProvider
{
    public ILayer[] layers = new ILayer[6];
    private static PlatformProvider instance;
    public static PlatformProvider Instance => instance ?? (instance = new PlatformProvider());
    
}
