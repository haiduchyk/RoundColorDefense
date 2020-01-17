using System;
using System.Collections;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class PositionStabilizer : MonoBehaviour
{
    private int AngleBetweenPlatforms;
    private int deltaAngelForAlignmentY;
    private float angleForXZRotation;
    private IRotateController rotateController;
    private new Camera camera;
    private Transform layerTransform;
    private SimplePlatformLayer layer;
    
    private float speedYRotation = 1.5f;
    private float speedXZRotation = 7;
    private int rightAngleY;
    private int rightAngleX;
    private int rightAngleZ;
    public int prevY;

    private float CurrentAngleY => layerTransform.eulerAngles.y;
    private float CurrentAngleX => layerTransform.eulerAngles.x;
    private float CurrentAngleZ => layerTransform.eulerAngles.z;
    private float CurrentAngleYWithAlignment => MathAngle.To360Degree(layerTransform.eulerAngles.y - deltaAngelForAlignmentY);
    public void Initialize()
    {
        layerTransform = transform;
        layer = layerTransform.GetComponent<SimplePlatformLayer>();
        AngleBetweenPlatforms = layer.AngelBetweenPlatforms;
        deltaAngelForAlignmentY = -AngleBetweenPlatforms / 2; 
        camera = Camera.main;
        prevY = (int) layerTransform.eulerAngles.y;
        if (AngleBetweenPlatforms == 120) rotateController = new RotateControllerForSecondLayer(layer);
        else rotateController = new SimpleRotateController(layer);
    }
    
    public IEnumerator StabilizeY()
    {

        EasyTouch.SetEnabled(false);
        FindNearestAngleY();

        while (true)
        {
            var condition = MathAngle.FirstAngleBiggerThenSecond(CurrentAngleY, rightAngleY - 1) &&
                             MathAngle.FirstAngleBiggerThenSecond(rightAngleY + 1, CurrentAngleY);
            if (condition)
            {
                layerTransform.eulerAngles = new Vector3(CurrentAngleX, rightAngleY, CurrentAngleZ);

                rotateController.ReshuffleAfterYRotation(prevY, rightAngleY, AngleBetweenPlatforms);
                prevY = rightAngleY;
                EasyTouch.SetEnabled(true);

                yield break;
            }
            var angle = GetAngleForYRotation();
            layerTransform.Rotate(0, angle, 0);
            
            yield return new WaitForFixedUpdate();
        }
    }

    private float GetAngleForYRotation()
    {
        var res = speedYRotation;
        if (MathAngle.FirstAngleBiggerThenSecond(CurrentAngleY, rightAngleY)) res *= -1;
        if (layer.isRotated) res *= -1;
        return res;
    }
   

    private void FindNearestAngleY()
    {
        var between = AngleBetweenPlatforms;
        var delta = deltaAngelForAlignmentY;
        if (between == 120) between /= 2;
        var leftOrRight = Math.Round((CurrentAngleYWithAlignment % between) / between);
        rightAngleY = (int) (leftOrRight + ((CurrentAngleYWithAlignment - (CurrentAngleYWithAlignment % between)) / between) ) * between;
        rightAngleY += delta;
        rightAngleY = MathAngle.To360Degree(rightAngleY);
    }
    public IEnumerator UpdateXZ(Platform platform)
    {
        EasyTouch.SetEnabled(false);
        var isRotated = layer.isRotated;
        var rotated = isRotated ? -1 : 1; 
        angleForXZRotation = MathAngle.To360Degree(platform.defaultAngleInLayer);
        angleForXZRotation = isRotated ? 360 - angleForXZRotation : angleForXZRotation;
        
        angleForXZRotation *= Mathf.Deg2Rad;
        var firstX = CurrentAngleX;
        var firstZ = CurrentAngleZ;
        var factorX = (float) -Math.Sin(angleForXZRotation) * rotated;
        var factorZ = (float) Math.Cos(angleForXZRotation) * rotated;
        while (true)
        {
            var speed = speedXZRotation;
            var angleX = speed * factorX;
            var angleZ = speed * factorZ;
            if (XZisRight(firstX, firstZ))
            {
                SetPerfectRotation();
                rotateController.ReshuffleAfterXZRotation(platform);
                layer.isRotated = !layer.isRotated;
                EasyTouch.SetEnabled(true);

                yield break;
            }
            layerTransform.Rotate(angleX, 0, 0, Space.Self);
            layerTransform.Rotate(0, 0, angleZ, Space.Self);
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator UpdateY()
    {
        var v = Input.mousePosition;
        v.z = 30;
        v = camera.ScreenToWorldPoint(v);
        var angleOfPlatform = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg + CurrentAngleY;
        while (true)
        {
            var touchPosition = Input.mousePosition;
            touchPosition.z = 30;
            touchPosition = camera.ScreenToWorldPoint(touchPosition);

            var angle = Mathf.Atan2(touchPosition.z, touchPosition.x) * Mathf.Rad2Deg - angleOfPlatform;
            angle = MathAngle.To360Degree(angle);
            layerTransform.rotation = Quaternion.Euler(CurrentAngleX, -angle, CurrentAngleZ);
            yield return new WaitForFixedUpdate();
        }
    }
    private void SetPerfectRotation()
    {
        var x = CurrentAngleX;
        var z = CurrentAngleZ;
        var leftOrRightX = Math.Round((x % 180) / 180);
        var leftOrRightZ = Math.Round((z % 180) / 180);
        rightAngleX = (int) (leftOrRightX + ((x - (x % 180)) / 180)) * 180;
        rightAngleZ = (int) (leftOrRightZ + ((z - (z % 180)) / 180)) * 180;
        rightAngleX = MathAngle.To360Degree(rightAngleX);
        rightAngleZ = MathAngle.To360Degree(rightAngleZ);
        FindNearestAngleY();
        prevY = rightAngleY;
        layerTransform.eulerAngles = new Vector3(rightAngleX, rightAngleY, rightAngleZ);
    }
    private bool XZisRight(float firstX, float firstZ)
    {
        var isRightX = MathAngle.AngleIsInRange(CurrentAngleX, firstX + 180 - 4, firstX + 180 + 4);
        if (isRightX)
        {
            var isZ0 = MathAngle.AngleIsInRange(CurrentAngleZ, - 4, 4);
            var isZ180 = MathAngle.AngleIsInRange(CurrentAngleZ, 180 - 4, 180 + 4);
            return isZ0 || isZ180;
        }
        var isRightZ = MathAngle.AngleIsInRange(CurrentAngleZ, firstZ + 180 - 4, firstZ + 180 + 4);
        if (isRightZ)
        {
            var isX0 = MathAngle.AngleIsInRange(CurrentAngleX, -4, 4);
            var isX180 = MathAngle.AngleIsInRange(CurrentAngleX, 180 - 4, 180 + 4);
            return isX0 || isX180;
        }
        return false;
    }
}
