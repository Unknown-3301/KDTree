using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Point;
    public int NumberOfPoints;

    Shape[] shapes;
    public static KDTree tree;
    void Start()
    { 

        float cameraSizeY = Camera.main.orthographicSize;
        float cameraSizeX = (16 / 9f) * cameraSizeY;

        shapes = new Shape[NumberOfPoints];
        for (int i = 0; i < NumberOfPoints; i++)
        {
            GameObject gameObject = Instantiate(Point, new Vector3(Random.Range(-cameraSizeX, cameraSizeX), Random.Range(-cameraSizeY, cameraSizeY)), Quaternion.identity);

            shapes[i] = gameObject.GetComponent<Point>().Shape;
        }

        tree = new KDTree(5, shapes);
    }

    void Update()
    {
        tree = new KDTree(5, shapes);
    }
}
