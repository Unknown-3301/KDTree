using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class KDTreeBranch<T> where T : class, IKDShape
{
    public bool Leaf { get => LowerBranch == null; }
    public int PartitionDimension { get; private set; }
    public float PartitionValue { get; private set; }
    public KDTreeBranch<T> Base { get; private set; }
    public KDTreeBranch<T> LowerBranch { get; private set; }
    public KDTreeBranch<T> HigherBranch { get; private set; }

    //These are for the elements volumes (areas in 2D), so all shapes must be fully included in that boundary.
    //These are dynamic and change depending of the shapes sizes.
    public float[] ShapeMinBoundary { get; private set; }
    public float[] ShapeMaxBoundary { get; private set; }
    public float[] ShapeBoundaryCenter { get; private set; }
    public float[] ShapeBoundarySize { get; private set; }

    //These are for the elements positions, so all shapes positions must be inside this boundary, otherwise they
    //considered 'moved outside this branch'. These are static and do not change unless the entire branch was rebuilt.
    public float[] PartitionMinBoundary { get; private set; }
    public float[] PartitionMaxBoundary { get; private set; }

    //The list is sorted by the PartitionDimension-th coordinate of every element.
    private List<T> storedShapes;
    private KDTreeSettings settings;
    private int dimensions;
    private int lowerElements;
    private int higherElements;

    public KDTreeBranch(KDTreeBranch<T> baseBranch, List<T> shapes, int dimensions, int partDimension, float[] partitionMin, float[] partitionMax, KDTreeSettings set)
    {
        Base = baseBranch;
        settings = set;
        PartitionDimension = partDimension;
        this.dimensions = dimensions;

        PartitionMinBoundary = partitionMin;
        PartitionMaxBoundary = partitionMax;
        ShapeMinBoundary = new float[dimensions];
        ShapeMaxBoundary = new float[dimensions];
        ShapeBoundaryCenter = new float[dimensions];
        ShapeBoundarySize = new float[dimensions];

        if (!set.StoreElementsInLeafsOnly || shapes.Count <= set.MaxLeafSize)
            storedShapes = shapes; //no copy is needed as 'shapes' was (temporarly) created and sorted in the base branch

        if (shapes.Count > set.MaxLeafSize)
            Split(shapes);

    }

    private void Split(List<T> sortedShapes)
    {
        lowerElements = sortedShapes.Count / 2;
        higherElements = sortedShapes.Count - sortedShapes.Count / 2;
        PartitionValue = sortedShapes[lowerElements].GetPosition(PartitionDimension);

        List<T> lowerList = sortedShapes.GetRange(0, lowerElements);
        List<T> higherList = sortedShapes.GetRange(lowerElements, higherElements);

        int newDim = PartitionDimension + 1 >= dimensions ? 0 : PartitionDimension + 1;

        lowerList.Sort((x1, x2) => Math.Sign(x1.GetPosition(newDim) - x2.GetPosition(newDim)));
        higherList.Sort((x1, x2) => Math.Sign(x1.GetPosition(newDim) - x2.GetPosition(newDim)));

        float[] lowerPartitionMin = new float[dimensions];
        float[] lowerPartitionMax = new float[dimensions];
        float[] higherPartitionMin = new float[dimensions];
        float[] higherPartitionMax = new float[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            lowerPartitionMin[i] = PartitionMinBoundary[i];
            higherPartitionMin[i] = PartitionMinBoundary[i];
            lowerPartitionMax[i] = PartitionMaxBoundary[i];
            higherPartitionMax[i] = PartitionMaxBoundary[i];
        }

        lowerPartitionMax[PartitionDimension] = PartitionValue;
        higherPartitionMin[PartitionDimension] = PartitionValue;


        if (LowerBranch == null)
            LowerBranch = new KDTreeBranch<T>(this, lowerList, dimensions, newDim, lowerPartitionMin, lowerPartitionMax, settings);
        else
            LowerBranch.Reset(lowerList, lowerPartitionMin, lowerPartitionMax);

        if (HigherBranch == null)
            HigherBranch = new KDTreeBranch<T>(this, higherList, dimensions, newDim, higherPartitionMin, higherPartitionMax, settings);
        else
            HigherBranch.Reset(higherList, higherPartitionMin, higherPartitionMax);
    }
    public (float[], float[]) CalculateShapeBoundaries()
    {
        if (Leaf)
        {
            for (int j = 0; j < dimensions; j++)
            {
                ShapeMinBoundary[j] = float.MaxValue;
                ShapeMaxBoundary[j] = float.MinValue;

                for (int i = 0; i < storedShapes.Count; i++)
                {
                    ShapeMinBoundary[j] = Math.Min(storedShapes[i].GetMin(j), ShapeMinBoundary[j]);
                    ShapeMaxBoundary[j] = Math.Max(storedShapes[i].GetMax(j), ShapeMaxBoundary[j]);

                    ShapeBoundaryCenter[j] = (ShapeMaxBoundary[j] + ShapeMinBoundary[j]) / 2;
                    ShapeBoundarySize[j] = (ShapeMaxBoundary[j] - ShapeMinBoundary[j]) / 2;
                }
            }
        }
        else
        {
            (float[] lowerMin, float[] lowerMax) = LowerBranch.CalculateShapeBoundaries();
            (float[] higherMin, float[] higherMax) = HigherBranch.CalculateShapeBoundaries();

            for (int i = 0; i < dimensions; i++)
            {
                ShapeMinBoundary[i] = Math.Min(lowerMin[i], higherMin[i]);
                ShapeMaxBoundary[i] = Math.Max(lowerMax[i], higherMax[i]);
                ShapeBoundaryCenter[i] = (ShapeMaxBoundary[i] + ShapeMinBoundary[i]) / 2;
                ShapeBoundarySize[i] = (ShapeMaxBoundary[i] - ShapeMinBoundary[i]) / 2;
            }
        }

        return (ShapeMinBoundary, ShapeMaxBoundary);
    }
    public void CheckForOutsiders(List<T> outsiders)
    {
        if (!Leaf)
        {
            LowerBranch.CheckForOutsiders(outsiders);
            HigherBranch.CheckForOutsiders(outsiders);
            
            return;
        }

        for (int i = 0; i < storedShapes.Count; i++)
        {
            if (!IsInside(storedShapes[i]))
            {
                outsiders.Add(storedShapes[i]);
                storedShapes[i].Deleted = true;
            }
        }
    }
    public int RemoveDeletedElements()
    {
        if (!Leaf)
        {
            int l = LowerBranch.RemoveDeletedElements();
            int h = HigherBranch.RemoveDeletedElements();

            lowerElements -= l;
            higherElements -= h;

            if (settings.StoreElementsInLeafsOnly)
                return l + h;
        }

        int num = storedShapes.RemoveAll(x => x.Deleted);

        return num;
    }
    public void AddElements(List<T> newElements)
    {
        if (newElements.Count == 0)
            return;

        if (Leaf)
        {
            storedShapes.AddRange(newElements);
            storedShapes.Sort(CompareElements);

            if (storedShapes.Count > settings.MaxLeafSize)
            {
                Split(storedShapes);

                if (settings.StoreElementsInLeafsOnly)
                    storedShapes = null;
            }

            return;
        }

        if (!settings.StoreElementsInLeafsOnly)
        {
            storedShapes.AddRange(newElements);
            storedShapes.Sort(CompareElements);
        }

        List<T> lowerElem = new List<T>(newElements.Count);
        List<T> higherElem = new List<T>(newElements.Count);

        for (int i = 0; i < newElements.Count; i++)
        {
            if (newElements[i].GetPosition(PartitionDimension) <= PartitionValue)
                lowerElem.Add(newElements[i]);
            else
                higherElem.Add(newElements[i]);
        }

        lowerElements += lowerElem.Count;
        higherElements += higherElem.Count;

        LowerBranch.AddElements(lowerElem);
        HigherBranch.AddElements(higherElem);
    }
    
    public void Restructure()
    {
        if (Leaf)
            return;

        int total = lowerElements + higherElements;
        float lowerPer = lowerElements / (float)total;
        float higherPer = higherElements / (float)total;
        float perDiff = Mathf.Abs(higherPer - lowerPer);

        if (perDiff >= settings.RebuildPercent)
        {
            List<T> sorted = GetAllElementsSorted();
            Reset(sorted, PartitionMinBoundary, PartitionMaxBoundary);
        }
        else
        {
            LowerBranch.Restructure();
            HigherBranch.Restructure();
        }
    }
    public void Reset(List<T> shapes, float[] partitionMin, float[] partitionMax)
    {
        if (!settings.StoreElementsInLeafsOnly || shapes.Count <= settings.MaxLeafSize)
            storedShapes = shapes; //no copy is needed as 'shapes' was (temporarly) created and sorted in the base branch

        PartitionMinBoundary = partitionMin;
        PartitionMaxBoundary = partitionMax;

        if (shapes.Count <= settings.MaxLeafSize)
        {
            LowerBranch = null;
            HigherBranch = null;
        }
        else
        {
            Split(shapes);
        }
    }
    public int GetGPUData<K>(GPUKDTreeData<K> gpuData, int baseIndex, Func<T, K> convert) where K : unmanaged
    {
        int myIndex = gpuData.Branches.Count;

        GPUTreeBranch data = new GPUTreeBranch()
        {
            BaseBranchIndex = baseIndex + gpuData.BranchsIndexOffset,
            LowerBranchIndex = 0,
            HigherBranchIndex = 0,
            PartitionDimension = PartitionDimension,
            PartitionValue = PartitionValue,
            Leaf = Leaf ? 1 : 0,
        };

        gpuData.Branches.Add(data);

        for (int i = 0; i < dimensions; i++)
        {
            gpuData.BranchPartitionBoundaries.Add(PartitionMinBoundary[i]);
        }
        for (int i = 0; i < dimensions; i++)
        {
            gpuData.BranchPartitionBoundaries.Add(PartitionMaxBoundary[i]);
        }
        for (int i = 0; i < dimensions; i++)
        {
            gpuData.BranchShapeBoundaries.Add(ShapeMinBoundary[i]);
        }
        for (int i = 0; i < dimensions; i++)
        {
            gpuData.BranchShapeBoundaries.Add(ShapeMaxBoundary[i]);
        }

        if (Leaf)
        {
            data.ElementsStartIndex = gpuData.OrderedElements.Count;
            data.ElementsCount = storedShapes.Count;

            for (int i = 0; i < storedShapes.Count; i++)
            {
                gpuData.OrderedElements.Add(convert(storedShapes[i]));
            }
        }
        else
        {
            int l = LowerBranch.GetGPUData(gpuData, myIndex, convert);
            int h = HigherBranch.GetGPUData(gpuData, myIndex, convert);

            data.LowerBranchIndex = l + gpuData.BranchsIndexOffset;
            data.HigherBranchIndex = h + gpuData.BranchsIndexOffset;
        }

        gpuData.Branches[myIndex] = data;

        return myIndex;
    }
    public bool IsInside(T shape)
    {
        for (int i = 0; i < dimensions; i++)
        {
            if (shape.GetPosition(i) < PartitionMinBoundary[i] || shape.GetPosition(i) > PartitionMaxBoundary[i])
                return false;
        }

        return true;
    }
    public List<T> GetAllElementsSorted()
    {
        if (!settings.StoreElementsInLeafsOnly || Leaf)
            return storedShapes;

        List<T> all = new List<T>(lowerElements + higherElements);
        AddAllElements(all);

        all.Sort(CompareElements);

        return all;
    }
    //This only needs to be run when settings.StoreElementsInLeafsOnly == true
    private void AddAllElements(List<T> elements)
    {
        if (!Leaf)
        {
            LowerBranch.AddAllElements(elements);
            HigherBranch.AddAllElements(elements);

            return;
        }

        elements.AddRange(storedShapes);
    }
    private int CompareElements(T x1, T x2) => Math.Sign(x1.GetPosition(PartitionDimension) - x2.GetPosition(PartitionDimension));

    public void FindNearestNeighbor(IKDShape shape, ref T closest, ref float sqrDistance)
    {
        if (Leaf)
        {
            for (int i = 0; i < storedShapes.Count; i++)
            {
                float sqr = KDSqrDistance(shape, storedShapes[i]);

                if (sqr < sqrDistance)
                {
                    sqrDistance = sqr;
                    closest = storedShapes[i];
                }
            }

            return;
        }

        float p = shape.GetPosition(PartitionDimension);
        KDTreeBranch<T> first = p > PartitionValue ? HigherBranch : LowerBranch;
        KDTreeBranch<T> second = p > PartitionValue ? LowerBranch : HigherBranch;

        first.FindNearestNeighbor(shape, ref closest, ref sqrDistance);

        if (second.GetBoxSquaredDistance(shape) >= sqrDistance)
            return;

        second.FindNearestNeighbor(shape, ref closest, ref sqrDistance);
    }
    public void FindNearestNeighbors(KNNsHolder<T> holder)
    {
        if (Leaf)
        {
            for (int i = 0; i < storedShapes.Count; i++)
            {
                if (storedShapes[i] == holder.Shape)
                    continue;
                 
                holder.TryAddNeighbor(storedShapes[i]);
            }

            return;
        }

        float p = holder.Shape.GetPosition(PartitionDimension);
        KDTreeBranch<T> first = p > PartitionValue ? HigherBranch : LowerBranch;
        KDTreeBranch<T> second = p > PartitionValue ? LowerBranch : HigherBranch;

        first.FindNearestNeighbors(holder);

        if (second.GetBoxSquaredDistance(holder.Shape) >= holder.SqrDistance)
            return;

        second.FindNearestNeighbors(holder);
    }
    public void GetCollidedShapes(IKDShape shape, List<T> collided)
    {
        if (Leaf)
        {
            for (int i = 0; i < storedShapes.Count; i++)
            {
                if (storedShapes[i] == shape)
                    continue;

                if (storedShapes[i].IsColliding(shape))
                    collided.Add(storedShapes[i]);
            }

            return;
        }

        bool lower = shape.IsCollidingBox(LowerBranch.ShapeBoundaryCenter, LowerBranch.ShapeBoundarySize);
        bool higher = shape.IsCollidingBox(HigherBranch.ShapeBoundaryCenter, HigherBranch.ShapeBoundarySize);

        if (!lower && !higher)
            return;

        bool both = lower && higher;

        KDTreeBranch<T> first = higher ? HigherBranch : LowerBranch;
        KDTreeBranch<T> second = both ? LowerBranch : null;

        first.GetCollidedShapes(shape, collided);
        second?.GetCollidedShapes(shape, collided);
    }
    float KDSqrDistance(IKDShape s1, IKDShape s2)
    {
        float sum = 0;

        for (int i = 0; i < dimensions; i++)
        {
            float diff = s1.GetPosition(i) - s2.GetPosition(i);
            sum += diff * diff;
        }

        return sum;
    }
    public float GetBoxSquaredDistance(IKDShape shape)
    {
        float sum = 0;

        for (int i = 0; i < dimensions; i++)
        {
            float p = shape.GetPosition(i);
            float d = p - Math.Min(Math.Max(PartitionMinBoundary[i], p), PartitionMaxBoundary[i]);
            sum += d * d;
        }

        return sum;
    }
}
