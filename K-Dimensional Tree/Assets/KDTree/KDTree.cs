using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KDTree<T> where T : class, IKDShape
{
    private KDTreeSettings settings;
    private int dimensions;
    public KDTreeBranch<T> MainBranch { get; private set; } 

    /// <summary>
    /// Creates new instance
    /// </summary>
    /// <param name="shapes">Tree elements.</param>
    /// <param name="dimensions">Tree dimension (also elements dimensions)</param>
    /// <param name="settings">Tree settings</param>
    public KDTree(List<T> shapes, int dimensions, KDTreeSettings settings)
    {
        this.settings = settings;
        this.dimensions = dimensions;

        float[] partMin = new float[dimensions];
        float[] partMax = new float[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            partMin[i] = float.MinValue;
            partMax[i] = float.MaxValue;
        }

        MainBranch = new KDTreeBranch<T>(null, shapes.OrderBy(x => x.GetPosition(0)).ToList(), dimensions, 0, partMin, partMax, settings);
        MainBranch.CalculateShapeBoundaries();
    }

    /// <summary>
    /// Updates the tree when ever elements change (move for example).
    /// </summary>
    public void Update()
    {
        List<T> elements = new List<T>();

        MainBranch.CheckForOutsiders(elements);
        MainBranch.RemoveDeletedElements();

        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].Deleted = false;
        }

        MainBranch.AddElements(elements);

        MainBranch.Restructure();
        MainBranch.CalculateShapeBoundaries();
    }

    /// <summary>
    /// Adds a collection of elements to the tree.
    /// </summary>
    /// <param name="newElements"></param>
    public void AddElements(List<T> newElements)
    {
        MainBranch.AddElements(newElements);
        MainBranch.Restructure();
        MainBranch.CalculateShapeBoundaries();
    }
    /// <summary>
    /// Remove a collection of elements from the tree.
    /// </summary>
    /// <param name="elements"></param>
    public void RemoveElements(List<T> elements)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].Deleted = true;
        }

        MainBranch.RemoveDeletedElements();
        MainBranch.Restructure();
        MainBranch.CalculateShapeBoundaries();

        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].Deleted = false;
        }
    }
    /// <summary>
    /// Returns the nearest neighbor to <paramref name="shape"/>.
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public T FindNearestNeighbor(IKDShape shape)
    {
        T closest = null;      
        float sqrDis = float.MaxValue;
        MainBranch.FindNearestNeighbor(shape, ref closest, ref sqrDis);

        return closest;
    }
    /// <summary>
    /// Returns the nearest <paramref name="neighborsNum"/> neighbors to <paramref name="shape"/>.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="neighborsNum"></param>
    /// <returns></returns>
    public T[] FindNearestNeighbors(IKDShape shape, int neighborsNum)
    {
        KNNsHolder<T> holder = new KNNsHolder<T>(neighborsNum, dimensions, shape);

        MainBranch.FindNearestNeighbors(holder);

        return holder.Neighbors;
    }
    /// <summary>
    /// Returns the collided shapes with <paramref name="shape"/>.
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public List<T> GetCollidedShapes(IKDShape shape)
    {
        List<T> collided = new List<T>();
        MainBranch.GetCollidedShapes(shape, collided);

        return collided;
    }

    /// <summary>
    /// Returns an instance containing data to be used in shaders.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <param name="convert">A function that converts tree elements into structs to be used in compute buffers in shaders.</param>
    /// <returns></returns>
    public GPUKDTreeData<K> ConvertToGPUData<K>(System.Func<T, K> convert) where K : unmanaged
    {
        GPUKDTreeData<K> data = new GPUKDTreeData<K>();

        MainBranch.GetGPUData(data, -1, convert);

        return data;
    }
}
