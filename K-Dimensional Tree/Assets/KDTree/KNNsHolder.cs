using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNNsHolder<T> where T : class, IKDShape
{
    public float SqrDistance { get => sqrDistances[sqrDistances.Length - 1]; }
    public IKDShape Shape { get; private set; }
    public T[] Neighbors { get; private set; }

    private int dimensions;
    private float[] sqrDistances;
    
    public KNNsHolder(int neighborsNum, int dimensions, IKDShape shape)
    {
        this.dimensions = dimensions;

        Shape = shape;
        Neighbors = new T[neighborsNum];
        sqrDistances = new float[neighborsNum];
        for (int i = 0; i < sqrDistances.Length; i++)
        {
            sqrDistances[i] = float.MaxValue;
        }
    }

    public void TryAddNeighbor(T neighbor)
    {
        for (int i = 0; i < Neighbors.Length; i++)
        {
            if (Neighbors[i] == null)
            {
                Neighbors[i] = neighbor;
                sqrDistances[i] = ShapesSqrDistance(neighbor, Shape);
                return;
            }
            else
            {
                if (neighbor == Neighbors[i])
                    return;

                float sqr = ShapesSqrDistance(neighbor, Shape);

                if (sqr < sqrDistances[i])
                {
                    ShiftNeighbors(i);

                    sqrDistances[i] = sqr;
                    Neighbors[i] = neighbor;

                    return;
                }
            }
        }
    }
    private void ShiftNeighbors(int start)
    {
        for (int i = Neighbors.Length - 1; i > start; i--)
        {
            Neighbors[i] = Neighbors[i - 1];
            sqrDistances[i] = sqrDistances[i - 1];
        }
    }
    private float ShapesSqrDistance(IKDShape shape, IKDShape otherShape)
    {
        float sum = 0;

        for (int i = 0; i < dimensions; i++)
        {
            float f = shape.GetPosition(i) - otherShape.GetPosition(i);
            sum += f * f;
        }

        return sum;
    }
}
