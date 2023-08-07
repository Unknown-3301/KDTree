using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : IKDShape
{
    public Vector2 Position { get; set; }
    
    public bool Deleted { get; set; }

    public float GetMax(int dimension) => GetPosition(dimension);

    public float GetMin(int dimension) => GetPosition(dimension);

    public float GetPosition(int dimension) => dimension == 0 ? Position.x : Position.y;

    public bool IsColliding(IKDShape other)
    {
        if (other.GetType() == typeof(SquareShape))
        {
            SquareShape s = (SquareShape)other;

            return Position.x >= s.Position.x - s.Size.x && Position.x <= s.Position.x + s.Size.x &&
                   Position.y >= s.Position.y - s.Size.y && Position.y <= s.Position.y + s.Size.y;
        }

        return false;
    }

    public bool IsCollidingBox(float[] position, float[] size)
    {
        return Position.x >= position[0] - size[0] && Position.x <= position[0] + size[0] &&
               Position.y >= position[1] - size[1] && Position.y <= position[1] + size[1];
    }
}
