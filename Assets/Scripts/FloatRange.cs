﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct FloatRange
{
    [SerializeField] private float min, max;

    public float Min => min;
    public float Max => max;

    public float RandomValueRange => Random.Range(min, max);

    public FloatRange(float value)
    {
        min = max = value;
    }

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max < min ? min : max;
    }
}
