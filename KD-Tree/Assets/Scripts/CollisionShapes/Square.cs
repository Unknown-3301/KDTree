using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : KDShape
{
    public float x, y;
    public float scaleX, scaleY;

    public Square(float x, float y)
    {
        MaxDimensions = 2;
        this.x = x;
        this.y = y;
    }

    public override int CompareTo(KDShape another, int dimension)
    {
        return (int)Mathf.Sign(GetPosition(dimension) - another.GetPosition(dimension));
    }

    public override float GetPosition(int dimension)
    {
        return dimension == 0 ? x : y;
    }

    public override float GetDistance(KDShape another)
    {
        if (another.GetType() == typeof(Square))
        {
            Square square = (Square)another;
            Vector2 squarePoint = new Vector2(Mathf.Clamp(x, square.x - square.scaleX, square.x + square.scaleX), Mathf.Clamp(y, square.y - square.scaleY, square.y + square.scaleY));

            Vector2 q = new Vector2(Mathf.Abs(squarePoint.x - x) - scaleX, Mathf.Abs(squarePoint.y - y) - scaleY);
            float maxQX = Mathf.Max(q.x, 0);
            float maxQY = Mathf.Max(q.y, 0);
            float dis = Mathf.Sqrt(maxQX * maxQX + maxQY * maxQY) + Mathf.Min(Mathf.Max(q.x, q.y), 0);

            return dis;
        }

        return 0;
    }

    public override bool IsEqualPosition(KDShape another)
    {
        return x == another.GetPosition(0) && y == another.GetPosition(1);
    }

    public override float DistanceToWall(float wall, int dimension)
    {
        Vector2 wallVec = new Vector2(dimension == 0 ? wall : x, dimension == 1 ? wall : y);

        Vector2 q = new Vector2(Mathf.Abs(wallVec.x - x) - scaleX, Mathf.Abs(wallVec.y - y) - scaleY);
        float maxQX = Mathf.Max(q.x, 0);
        float maxQY = Mathf.Max(q.y, 0);
        float dis = Mathf.Sqrt(maxQX * maxQX + maxQY * maxQY) + Mathf.Min(Mathf.Max(q.x, q.y), 0);

        return dis;
    }
}
