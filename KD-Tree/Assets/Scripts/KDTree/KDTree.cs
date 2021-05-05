using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTree
{
    private KDTree smallerBranch;
    private KDTree biggerBranch;

    private int dimensionalDepth;
    private int limit;
    private float median;

    private List<KDShape> shapes;

    /// <summary>
    /// Creates now KDTree class.
    /// </summary>
    /// <param name="shapeLimit">the maximum number of shapes in one partition. (for default 4?)</param>
    public KDTree(int shapeLimit)
    {
        limit = shapeLimit;
        shapes = new List<KDShape>();
    }
    /// <summary>
    /// Creates now KDTree class.
    /// </summary>
    /// <param name="shapeLimit">the maximum number of shapes in one partition. (for default 4?)</param>
    /// <param name="startShapes">the array of shapes.</param>
    public KDTree(int shapeLimit, KDShape[] startShapes)
    {
        limit = shapeLimit;
        shapes = new List<KDShape>(startShapes.Length);
        for (int i = 0; i < startShapes.Length; i++)
        {
            shapes.Add(startShapes[i]);
        }

        if (shapes.Count > limit)
        {
            Split();
        }
    }
    /// <summary>
    /// Creates now KDTree class.
    /// </summary>
    /// <param name="shapeLimit">the maximum number of shapes in one partition. (for default 4?)</param>
    /// <param name="startShapes">the list of shapes.</param>
    public KDTree(int shapeLimit, List<KDShape> startShapes)
    {
        limit = shapeLimit;
        shapes = new List<KDShape>(startShapes.Count);
        for (int i = 0; i < startShapes.Count; i++)
        {
            shapes.Add(startShapes[i]);
        }

        if (shapes.Count > limit)
        {
            Split();
        }
    }
    private KDTree(int shapeLimit, List<KDShape> startShapes, int _dimensionalDepth)
    {
        limit = shapeLimit;
        shapes = startShapes;
        dimensionalDepth = _dimensionalDepth;

        if (shapes.Count > limit)
        {
            Split();
        }
    }

    /// <summary>
    /// Adds a shape to the list of shapes.
    /// </summary>
    /// <param name="shape">the shape. NOTE its position must be uniqe were there arent 2 points or more ar in the same position exactly.</param>
    public void AddShape(KDShape shape)
    {
        if (!IsLeaf())
        {
            (shape.GetPosition(dimensionalDepth) <= median ? smallerBranch : biggerBranch).AddShape(shape);
        }
        else
        {
            if (!shapes.Any(x => x.IsEqualPosition(shape)))
            {
                shapes.Add(shape);
                if (shapes.Count > limit)
                {
                    Split();
                }
            }
        }
    }
    /// <summary>
    /// Returns all the shapes that thier distance to "shape" is less than "distance".
    /// </summary>
    /// <param name="shape">the shape.</param>
    /// <param name="distance">the distance.</param>
    /// <returns></returns>
    public List<KDShape> GetClosestNeighborsInRange(KDShape shape, float distance)
    {
        List<KDShape> closeShapes = new List<KDShape>();

        if (!IsLeaf())
        {
            float currentPos = shape.GetPosition(dimensionalDepth);
            bool smaller = currentPos <= median;

            (smaller ? smallerBranch : biggerBranch).GetClosestNeighborsInRange(closeShapes, shape, distance * distance);

            if (shape.DistanceToWall(median, dimensionalDepth) >= distance * distance)
                return closeShapes;

            (smaller ? biggerBranch : smallerBranch).GetClosestNeighborsInRange(closeShapes, shape, distance * distance);
        }
        else
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                 if (shapes[i] != shape)
                {
                    float dis = shape.GetDistance(shapes[i]);
                    if (dis < distance * distance)
                    {
                        closeShapes.Add(shapes[i]);
                    }
                }
            }
        }

        return closeShapes;
    }
    private void GetClosestNeighborsInRange(List<KDShape> closeShapes, KDShape shape, float sqrdistance)
    {
        if (!IsLeaf())
        {
            float currentPos = shape.GetPosition(dimensionalDepth);
            bool smaller = currentPos <= median;

            (smaller ? smallerBranch : biggerBranch).GetClosestNeighborsInRange(closeShapes, shape, sqrdistance);

            if (shape.DistanceToWall(median, dimensionalDepth) >= sqrdistance)
                return;

            (smaller ? biggerBranch : smallerBranch).GetClosestNeighborsInRange(closeShapes, shape, sqrdistance);
        }
        else
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i] != shape)
                {
                    float dis = shape.GetDistance(shapes[i]);
                    if (dis < sqrdistance)
                    {
                        closeShapes.Add(shapes[i]);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Return the closest shape to "shape".
    /// </summary>
    /// <param name="shape">the shape.</param>
    /// <returns></returns>
    public KDShape GetClosestNeighbor(KDShape shape)
    {
        KDShape closeShape = null;
        float closeDistance = float.NaN;

        if (!IsLeaf())
        {
            float currentPos = shape.GetPosition(dimensionalDepth);
            bool smaller = currentPos <= median;

            closeShape = (smaller ? smallerBranch : biggerBranch).GetClosestNeighbor(shape, ref closeDistance);

            if (closeShape != null)
            {
                if (shape.DistanceToWall(median, dimensionalDepth) >= closeDistance)
                    return closeShape;
            }

            KDShape otherClose = (smaller ? biggerBranch : smallerBranch).GetClosestNeighbor(shape, ref closeDistance);

            if (otherClose != null)
                closeShape = otherClose;
        }
        else
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i] != shape)
                {
                    if (float.IsNaN(closeDistance))
                    {
                        closeDistance = shape.GetDistance(shapes[i]);
                        closeShape = shapes[i];
                    }
                    else
                    {
                        float dis = shape.GetDistance(shapes[i]);
                        if (dis < closeDistance)
                        {
                            closeDistance = dis;
                            closeShape = shapes[i];
                        }
                    }
                }
            }
        }

        return closeShape;
    }
    private KDShape GetClosestNeighbor(KDShape shape, ref float closeDistance)
    {
        KDShape closeShape = null;
        if (!IsLeaf())
        {
            float currentPos = shape.GetPosition(dimensionalDepth);
            bool smaller = currentPos <= median;

            closeShape = (smaller ? smallerBranch : biggerBranch).GetClosestNeighbor(shape, ref closeDistance);

            if (closeShape != null)
            {
                if (shape.DistanceToWall(median, dimensionalDepth) >= closeDistance)
                    return closeShape;
            }

            KDShape otherClose = (smaller ? biggerBranch : smallerBranch).GetClosestNeighbor(shape, ref closeDistance);

            if (otherClose != null)
                closeShape = otherClose;
        }
        else
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i] != shape)
                {
                    if (float.IsNaN(closeDistance))
                    {
                        closeDistance = shape.GetDistance(shapes[i]);
                        closeShape = shapes[i];
                    }
                    else
                    {
                        float dis = shape.GetDistance(shapes[i]);
                        if (dis < closeDistance)
                        {
                            closeDistance = dis;
                            closeShape = shapes[i];
                        }
                    }
                }
            }
        }

        return closeShape;
    }
    private bool IsLeaf()
    {
        return smallerBranch == null;
    }
    private void Split()
    {
        median = GetMedian(shapes, dimensionalDepth);

        List<KDShape> smaller = shapes.FindAll(x => x.GetPosition(dimensionalDepth) <= median);
        List<KDShape> bigger = shapes.FindAll(x => x.GetPosition(dimensionalDepth) > median);

        int newDepth = (dimensionalDepth + 1) % shapes[0].MaxDimensions;

        smallerBranch = new KDTree(limit, smaller, newDepth);
        biggerBranch = new KDTree(limit, bigger, newDepth);
    }
    private float GetMedian(List<KDShape> shapes, int dimension)
    {
        shapes.Sort((first, second) => first.CompareTo(second, dimension));

        if (shapes.Count % 2 != 0)
        {
            return shapes[shapes.Count / 2].GetPosition(dimension);
        }
        else
        {
            return (shapes[shapes.Count / 2].GetPosition(dimension) + shapes[shapes.Count / 2 - 1].GetPosition(dimension)) / 2f;
        }
    }
}
