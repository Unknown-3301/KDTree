using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GPUColoredPointData
{
    public Vector2 Position;
    public Color Color;

    public static int Size { get => sizeof(float) * 6; }
}
