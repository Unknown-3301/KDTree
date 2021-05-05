using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionShapesManager : MonoBehaviour
{
    Square[] squares;
    public static KDTree Tree;

    void Start()
    {
        CollisionObject[] collisionObjects = FindObjectsOfType<CollisionObject>();
        squares = new Square[collisionObjects.Length];
        for (int i = 0; i < squares.Length; i++)
        {
            squares[i] = collisionObjects[i].square;
        }

        Tree = new KDTree(4, squares);

        Debug.Log(Tree);
    }

    void Update()
    {   
        Tree = new KDTree(4, squares);
    }
}
