using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] private bool showKDTree;
    [SerializeField] private bool useKDTree;
    [SerializeField] private GameObject dotsPrefab;
    [SerializeField] private GameObject linePrefab;

    public static Rect ScreenRect { get; private set; }
    public readonly static int Dots = 130;
    public readonly static int Connections = 7;

    private static Test1 instance;

    private KDTree<Point> tree;
    private List<Point> points;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this);
            return;
        }

        Vector2 pos = Camera.main.transform.position;
        Vector2 size = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);

        ScreenRect = new Rect(pos - size, size * 2);

        List<Point> points = new List<Point>(Dots);
        for (int i = 0; i < Dots; i++)
        {
            Vector2 pPos = new Vector2(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y)) + pos;
            points.Add(Instantiate(dotsPrefab, pPos, Quaternion.identity).GetComponent<Dot>().Point);
        }

        this.points = points;

        tree = new KDTree<Point>(points, 2, new KDTreeSettings()
        {
            MaxLeafSize = 5,
            RebuildPercent = 0.4f,
            StoreElementsInLeafsOnly = false
        });
    }
    private void Update()
    {
        if (useKDTree)
        {
            //tree = new KDTree<Point>(points, 2, new KDTreeSettings()
            //{
            //    MaxLeafSize = 5,
            //    RebuildPercent = 0.4f,
            //    StoreElementsInLeafsOnly = false
            //});
            tree.Update();
        }
    }
    public static LineConnector CreateLine(Dot from, Point to)
    {
        LineConnector l = Instantiate(instance.linePrefab, from.transform).GetComponent<LineConnector>();

        l.From = from.Point;
        l.To = to;

        return l;
    }
    public static Point[] GetNearestPoints(Point p)
    {
        if (instance.useKDTree)
        {
            return instance.tree.FindNearestNeighbors(p, Connections);
        }
        else
        {
            KNNsHolder<Point> h = new KNNsHolder<Point>(Connections, 2, p);

            for (int i = 0; i < Dots; i++)
            {
                if (instance.points[i] == p)
                    continue;

                h.TryAddNeighbor(instance.points[i]);
            }

            return h.Neighbors;
        }
    }
}
