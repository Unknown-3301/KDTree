using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test3 : MonoBehaviour
{
    [SerializeField] private ComputeShader shader;
    [SerializeField] private GameObject pointsPrefab;

    public static Rect ScreenRect { get; private set; }
    public static int PointsNum = 130;

    private static Test3 instance;

    private KDTree<ColoredPoint> tree;
    private List<ColoredPoint> points;

    private ComputeBuffer branches;
    private ComputeBuffer branchesPosition;
    private ComputeBuffer pointsData;

    private RenderTexture texture;

    private GPUKDTreeData<GPUColoredPointData> gpuData;

    private ColoredPoint mouseRegionPoint;

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

        List<ColoredPoint> points = new List<ColoredPoint>(PointsNum);

        for (int i = 0; i < PointsNum; i++)
        {
            Vector2 pPos = new Vector2(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y)) + pos;
            points.Add(Instantiate(pointsPrefab, pPos, Quaternion.identity).GetComponent<MovingPoint>().Point);
        }
        
        this.points = points;

        tree = new KDTree<ColoredPoint>(points, 2, new KDTreeSettings()
        {
            MaxLeafSize = 5,
            RebuildPercent = 0.15f,
            StoreElementsInLeafsOnly = false,
        });

        gpuData = tree.ConvertToGPUData(x => x.Convert());

        branches = new ComputeBuffer(gpuData.Branches.Count, GPUTreeBranch.Size);
        branchesPosition = new ComputeBuffer(gpuData.Branches.Count * 4, sizeof(float));
        pointsData = new ComputeBuffer(points.Count, GPUColoredPointData.Size);

        branches.SetData(gpuData.Branches);
        branchesPosition.SetData(gpuData.BranchShapeBoundaries);
        pointsData.SetData(gpuData.OrderedElements);

        texture = new RenderTexture(Screen.width, Screen.height, 1);
        texture.enableRandomWrite = true;
        texture.Create();
    }

    void ChangeRegionColor()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        ColoredPoint p = tree.FindNearestNeighbor(new Point() { Position = mousePos });

        if (mouseRegionPoint == null)
            mouseRegionPoint = p;

        if (mouseRegionPoint != p)
        {
            mouseRegionPoint.ResetColor();
        }

        mouseRegionPoint = p;

        Vector3 n = new Vector3(Mathf.Cos(Time.time), Mathf.Cos(Time.time + 2), Mathf.Cos(Time.time + 4)) * 0.5f;
        n.x += 0.5f;
        n.y += 0.5f;
        n.z += 0.5f;

        mouseRegionPoint.Color = new Color(n.x, n.y, n.z, 1);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.Space))
        {
            List<ColoredPoint> points = new List<ColoredPoint>(PointsNum);
            Vector2 pos = ScreenRect.center;
            Vector2 size = ScreenRect.size / 2;

            int p = 10;

            for (int i = 0; i < p; i++)
            {
                Vector2 pPos = new Vector2(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y)) + pos;
                points.Add(Instantiate(pointsPrefab, pPos, Quaternion.identity).GetComponent<MovingPoint>().Point);
            }

            tree.AddElements(points);

            this.points.AddRange(points);

            PointsNum += p;
        }

        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.Space))
        {
            List<ColoredPoint> points = new List<ColoredPoint>(PointsNum);

            int p = 10;

            for (int i = 0; i < p; i++)
            {
                points.Add(this.points[this.points.Count - 1]);
                this.points.RemoveAt(this.points.Count - 1);
            }

            tree.RemoveElements(points);

            PointsNum -= p;
        }

        tree.Update();

        ChangeRegionColor();

        gpuData = tree.ConvertToGPUData(x => x.Convert());

        if (branches.count != gpuData.Branches.Count)
        {
            branches.Dispose();
            branchesPosition.Dispose();
            branches = new ComputeBuffer(gpuData.Branches.Count, GPUTreeBranch.Size);
            branchesPosition = new ComputeBuffer(gpuData.Branches.Count * 4, sizeof(float));
        }

        if (pointsData.count != PointsNum)
        {
            pointsData.Dispose();
            pointsData = new ComputeBuffer(PointsNum, GPUColoredPointData.Size);
        }

        branches.SetData(gpuData.Branches);
        branchesPosition.SetData(gpuData.BranchPartitionBoundaries);
        pointsData.SetData(gpuData.OrderedElements);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int kernel = shader.FindKernel("CSMain");

        shader.SetBuffer(kernel, "Points", pointsData);
        shader.SetBuffer(kernel, "Branches", branches);
        shader.SetBuffer(kernel, "BranchesPositions", branchesPosition);

        Vector2 d = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        shader.SetVector("ScreenToWorld", d);

        shader.SetTexture(kernel, "Texture", texture);

        shader.Dispatch(kernel, Mathf.CeilToInt(texture.width / 8f), Mathf.CeilToInt(texture.height / 8f), 1);

        Graphics.Blit(texture, destination);
    }

    private void OnDestroy()
    {
        branches.Dispose();
        branchesPosition.Dispose();
        pointsData.Dispose();
    }
}
