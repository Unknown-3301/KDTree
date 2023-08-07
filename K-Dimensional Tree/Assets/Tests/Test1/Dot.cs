using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public Point Point { get; private set; }

    private LineConnector[] connectors;

    private Vector2 dir;
    private float speed = 2f;

    private void Awake()
    {
        Point = new Point();
        Point.Position = transform.position;
        dir = Random.insideUnitCircle.normalized;
    }

    void Start()
    {
        connectors = new LineConnector[Test1.Connections];
        for (int i = 0; i < connectors.Length; i++)
        {
            connectors[i] = Test1.CreateLine(this, Point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Point.Position = transform.position;
        Point[] ps = Test1.GetNearestPoints(Point);

        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i] == null)
                break;

            connectors[i].To = ps[i];
        }

        Vector2 newPos = (Vector2)transform.position + dir * speed * Time.deltaTime;
        Vector2 clampedPos = new Vector2(Mathf.Clamp(newPos.x, Test1.ScreenRect.xMin, Test1.ScreenRect.xMax), Mathf.Clamp(newPos.y, Test1.ScreenRect.yMin, Test1.ScreenRect.yMax));

        if (newPos != clampedPos)
        {
            dir = Random.insideUnitCircle.normalized;
        }

        transform.position = clampedPos;
        Point.Position = clampedPos;
    }
}
