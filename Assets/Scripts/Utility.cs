﻿using System;
using UnityEngine;


public static class Utility
{
    public static Vector2 Rotate2D(Vector2 v, float deltaDegrees)
    {
        var angleRadians = deltaDegrees * (float) Math.PI / 180.0f;
        return new Vector2(
            v.x * Mathf.Cos(angleRadians) - v.y * Mathf.Sin(angleRadians),
            v.x * Mathf.Sin(angleRadians) + v.y * Mathf.Cos(angleRadians)
        );
    }

    public static float ToDegrees(float radians)
    {
        return radians * 180 / Mathf.PI;
    }

    public static float ToRadians(float degrees)
    {
        return (Mathf.PI / 180) * degrees;
    }
}
