using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPUKDTreeData<T> where T : unmanaged
{
    /// <summary>
    /// The list containing branches info to be used in a computer buffer.
    /// Note: the order of the branches in the list must not be changed.
    /// </summary>
    public List<GPUTreeBranch> Branches { get; private set; }
    /// <summary>
    /// Contains the coordinates for branches partition boundaries (min first then max), where every n floats represents a <see cref="KDTreeBranch{T}.PartitionMinBoundary"/> or <see cref="KDTreeBranch{T}.PartitionMaxBoundary"/>
    /// where n is the number of dimensions.
    /// </summary>
    public List<float> BranchPartitionBoundaries { get; private set; }
    /// <summary>
    /// Contains the coordinates for branches partition boundaries (min first then max), where every n floats represents a <see cref="KDTreeBranch{T}.ShapeMinBoundary"/> or <see cref="KDTreeBranch{T}.ShapeMaxBoundary"/>
    /// where n is the number of dimensions.
    /// </summary>
    public List<float> BranchShapeBoundaries { get; private set; }
    /// <summary>
    /// The elements of the tree ordered to be used in a computer buffer.
    /// Note: the order of the elements must not change when getting elements info to a struct (to be imported to a computer buffer).
    /// </summary>
    public List<T> OrderedElements { get; private set; }

    public int BranchsIndexOffset { get; set; }

    public GPUKDTreeData()
    {
        Branches = new List<GPUTreeBranch>();
        BranchPartitionBoundaries = new List<float>();
        BranchShapeBoundaries = new List<float>();
        OrderedElements = new List<T>();
    }
}
