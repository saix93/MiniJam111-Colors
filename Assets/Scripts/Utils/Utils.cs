using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{

}

[Serializable]
public struct MinMaxFloat
{
    public float Min;
    public float Max;

    public float random { get { return Random.Range(Min, Max); } }

    public MinMaxFloat(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float Lerp(float t)
    {
        return Mathf.Lerp(Min, Max, t);
    }

    public float InverseLerp(float value)
    {
        return Mathf.InverseLerp(Min, Max, value);
    }

    public float Clamp(float value)
    {
        return Mathf.Clamp(value, Min, Max);
    }

    /// <summary>
    /// El valor medio entre Min y Max
    /// </summary>
    public float Mean => Min + Max / 2;
    /// <summary>
    /// Longitud entre Min y Max. Siempre positiva
    /// </summary>
    public float Length => Mathf.Abs(Max - Min);
}

[Serializable]
public struct MinMaxInt
{
    public int Min;
    public int Max;

    public float random { get { return Random.Range(Min, Max); } }
}
