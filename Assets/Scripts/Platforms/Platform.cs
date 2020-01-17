using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class Platform : MonoBehaviour
{
    public int defaultAngleInLayer;
    public int indexOfLayer;
    public List<Enemy> enemies = new List<Enemy>();
    public GameObject layer;
    public virtual bool isTrap { get; set; }
    public float CurrentAngle => MathAngle.To360Degree(defaultAngleInLayer - transform.parent.rotation.eulerAngles.y);
    protected PlatformState.Type state = PlatformState.Type.Simple;
    public PlatformState.Type State => state;
    
}
