using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : KDShape
{
    public float x, y;
    public float realX, realY;

    public Shape(float _x, float _y)
    {
        MaxDimensions = 2;
        x = _x;
        y = _y;
    }

    public override int CompareTo(KDShape other, int dimension)
    {
        if (dimension == 0)
        {
            return (int)Mathf.Sign(x - other.GetPosition(dimension));
        }
        else if (dimension == 1)
        {
            return (int)Mathf.Sign(y - other.GetPosition(dimension));
        }
        else
        {
            throw new Exception("There is not more dimensions to get");
        }
    }
    public override float GetPosition(int dimension)
    {
        if (dimension == 0)
        {
            return x;
        }
        else if (dimension == 1)
        {
            return y;
        }
        else
        {
            throw new Exception("There is not more dimensions to get");
        }
    }
    public override float GetDistance(KDShape another)
    {
        if (another.MaxDimensions < MaxDimensions)
        {
            float _x = x - another.GetPosition(0);
            return _x * _x;
        }
        else
        {
            return new Vector2(x - another.GetPosition(0), y - another.GetPosition(1)).sqrMagnitude;
        }
    }
    public override bool IsEqualPosition(KDShape another)
    {
        return x == another.GetPosition(0) && y == another.GetPosition(1);
    }

    public override float DistanceToWall(float wall, int dimension)
    {
        Vector2 wallVec = new Vector2(dimension == 0 ? wall : x, dimension == 1 ? wall : y);
        Vector2 pos = new Vector2(x, y);

        return (wallVec - pos).sqrMagnitude;
    }
}
