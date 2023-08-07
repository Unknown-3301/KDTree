using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPoint : MonoBehaviour
{
    public ColoredPoint Point { get; private set; }

    private Vector2 dir;
    private float speed = 1f;

    private void Awake()
    {
        Point = new ColoredPoint();
        Point.InitColor = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);
        Point.ResetColor();
        Point.Position = transform.position;
        dir = Random.insideUnitCircle.normalized;
    }

    
    void Update()
    {
        Vector2 newPos = Point.Position + dir * speed * Time.deltaTime;
        Vector2 clampedPos = new Vector2(Mathf.Clamp(newPos.x, Test3.ScreenRect.xMin, Test3.ScreenRect.xMax), Mathf.Clamp(newPos.y, Test3.ScreenRect.yMin, Test3.ScreenRect.yMax));

        if (newPos != clampedPos)
        {
            dir = Random.insideUnitCircle.normalized;
        }

        Point.Position = clampedPos;
    }
}
