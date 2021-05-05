using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObject : MonoBehaviour
{
    public Square square;
    SpriteRenderer renderer;
    float dir;
    void Awake()
    {
        square = new Square(transform.position.x, transform.position.y);
        square.scaleX = transform.localScale.x / 2f;
        square.scaleY = transform.localScale.y / 2f;

        renderer = GetComponent<SpriteRenderer>();

        dir = Random.Range(0, 360f);
    }

    void Update()
    {
        KDShape shape = CollisionShapesManager.Tree.GetClosestNeighbor(square);

        if (shape != null)
        {
            if (square.GetDistance(shape) <= 0)
            {
                renderer.color = Color.white;
            }
            else
            {
                renderer.color = Color.black;
            }
        }

        float speed = 1f * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(Mathf.Cos(dir), Mathf.Sin(dir)) * speed;
        Vector3 pixelNewPos = Camera.main.WorldToScreenPoint(newPos);
        if (pixelNewPos.x < 0 || pixelNewPos.x >= Screen.width || pixelNewPos.y < 0 || pixelNewPos.y >= Screen.height)
        {
            dir = Random.Range(0, 360f);
        }
        else
        {
            transform.position = newPos;
        }

        square.x = transform.position.x;
        square.y = transform.position.y;
    }
}
