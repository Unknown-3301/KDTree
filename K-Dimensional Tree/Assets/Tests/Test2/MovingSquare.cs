using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSquare : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;

    public SquareShape Square { get; set; }
    
    private Color color;
    private Vector2 dir;
    private float speed = 2f;

    private void Awake()
    {
        color = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);
        Square = new SquareShape();
        Square.Position = transform.position;
        Square.Size = transform.localScale / 2;
        dir = Random.insideUnitCircle.normalized;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<SquareShape> collided = Test2.GetCollided(Square);

        if (collided.Count == 0)
        {
            renderer.color = Color.black;
        }
        else
        {
            renderer.color = color;
        }

        Vector2 newPos = (Vector2)transform.position + dir * speed * Time.deltaTime;
        Vector2 min = Test2.ScreenRect.min + Square.Size;
        Vector2 max = Test2.ScreenRect.max - Square.Size;
        Vector2 clampedPos = new Vector2(Mathf.Clamp(newPos.x, min.x, max.x), Mathf.Clamp(newPos.y, min.y, max.y));

        if (newPos != clampedPos)
        {
            dir = Random.insideUnitCircle.normalized;
        }

        transform.position = clampedPos;
        Square.Position = clampedPos;
    }
}
