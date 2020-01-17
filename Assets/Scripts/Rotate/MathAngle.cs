using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathAngle
{
    public static float To360Degree(float angle) => (angle + 360) % 360;
    public static int To360Degree(int angle) => (angle + 360) % 360;
    public static bool AngleIsInRange(float angle, float firstLimit, float secondLimit)
    {
        return FirstAngleBiggerThenSecond(angle, firstLimit) && 
               FirstAngleBiggerThenSecond(secondLimit, angle);
    }

    public static bool FirstAngleBiggerThenSecond(float first, float second)
    {
        // 270 200 / 340 10 / 271 90
        first = To360Degree(first);
        second = To360Degree(second);
        if (first > second) return (first - second < 180);
        return (second - first > 180);
    }
}
