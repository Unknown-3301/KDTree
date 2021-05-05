using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Shape Shape;

    private void Awake()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Shape = new Shape(screenPos.x, screenPos.y);
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
    }
    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        Shape.x = screenPos.x;
        Shape.y = screenPos.y;
        Shape.realX = transform.position.x;
        Shape.realY = transform.position.y;

        float speed = 1f * Time.deltaTime;
        Vector3 newPos = transform.position + transform.right * speed;
        Vector3 pixelNewPos = Camera.main.WorldToScreenPoint(newPos);
        if (pixelNewPos.x < 0 || pixelNewPos.x >= Screen.width || pixelNewPos.y < 0 || pixelNewPos.y >= Screen.height)
        {
            transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
        }
        else
        {
            transform.position = newPos;
        }
    }

    private void OnDrawGizmos()
    {
        if (GameManager.tree != null)
        {
            #region Range Search
            List<KDShape> closests = GameManager.tree.GetClosestNeighborsInRange(Shape, 200);

            float viewField = 180;
            float rotateSpeed = 1 * Time.deltaTime;
            Vector2 right = transform.right;

            for (int i = 0; i < closests.Count; i++)
            {
                Shape closest = (Shape)closests[i];


                // just for visual effect (were the shape try to dodge other shapes in its direction) so they spread evenly and wont center in spots
                Vector2 dirToTarget = new Vector2(closest.realX - Shape.realX, closest.realY - Shape.realY);
                float angle = Vector2.SignedAngle(right, dirToTarget);
                if (Mathf.Abs(angle) <= viewField / 2f)
                {
                    transform.eulerAngles += new Vector3(0, 0, -angle * rotateSpeed);
                }



                Gizmos.color = new Color(0.9f, 0.9f, 0.9f);
                Gizmos.DrawLine(new Vector3(Shape.realX, Shape.realY, 1), new Vector3(closest.realX, closest.realY, 1));
            }
            #endregion


            #region Closest Search
            //Shape closest = (Shape)GameManager.tree.GetClosestNeighbor(Shape);

            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(new Vector3(Shape.realX, Shape.realY, 1), new Vector3(closest.realX, closest.realY, 1));
            #endregion
        }
    }
}
