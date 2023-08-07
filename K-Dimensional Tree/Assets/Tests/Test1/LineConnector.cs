using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    [SerializeField] private LineRenderer renderer;

    public Point From { get; set; }
    public Point To { get; set; }

    private void Update()
    {
        renderer.SetPosition(0, From.Position);
        renderer.SetPosition(1, To.Position);
    }
}
