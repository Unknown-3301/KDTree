using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    [SerializeField] private bool useKDTree;
    [SerializeField] private GameObject squarePrefab;

    public static Rect ScreenRect { get; private set; }
    public readonly static int SquaresNum = 530;

    private static Test2 instance;

    private KDTree<SquareShape> tree;
    private List<SquareShape> squares;

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

        List<SquareShape> squares = new List<SquareShape>(SquaresNum);
        for (int i = 0; i < SquaresNum; i++)
        {
            Vector2 pPos = new Vector2(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y)) + pos;
            squares.Add(Instantiate(squarePrefab, pPos, Quaternion.identity).GetComponent<MovingSquare>().Square);
        }

        this.squares = squares;

        tree = new KDTree<SquareShape>(squares, 2, new KDTreeSettings()
        {
            MaxLeafSize = 5,
            RebuildPercent = 0.15f,
            StoreElementsInLeafsOnly = false,
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (useKDTree)
        {
            tree.Update();
        }
    }

    public static List<SquareShape> GetCollided(SquareShape square)
    {
        if (instance.useKDTree)
            return instance.tree.GetCollidedShapes(square);
        else
        {
            List<SquareShape> c = new List<SquareShape>();
            for (int i = 0; i < SquaresNum; i++)
            {
                if (instance.squares[i] == square)
                    continue;

                if (square.IsColliding(instance.squares[i]))
                    c.Add(instance.squares[i]);
            }

            return c;
        }
    }
}
