using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareShape : IKDShape
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    public bool Deleted { get; set; }

    public float GetMax(int dimension) => dimension == 0 ? Position.x + Size.x : Position.y + Size.y;

    public float GetMin(int dimension) => dimension == 0 ? Position.x - Size.x : Position.y - Size.y;

    public float GetPosition(int dimension) => dimension == 0 ? Position.x: Position.y;

    public bool IsColliding(IKDShape other)
    {
        if (other.GetType() == typeof(SquareShape))
        {
            SquareShape otherS = (SquareShape)other;

            Vector2 diff = new Vector2(Mathf.Abs(otherS.Position.x - Position.x), Mathf.Abs(otherS.Position.y - Position.y));
            return diff.x < Size.x + otherS.Size.x && diff.y < Size.y + otherS.Size.y;
        }

        return false;
    }

    public bool IsCollidingBox(float[] position, float[] size)
    {
        Vector2 sPos = new Vector2(position[0], position[1]);
        Vector2 sSize = new Vector2(size[0], size[1]);

        Vector2 diff = new Vector2(Mathf.Abs(sPos.x - Position.x), Mathf.Abs(sPos.y - Position.y));
        return diff.x < Size.x + sSize.x && diff.y < Size.y + sSize.y;
    }
}
