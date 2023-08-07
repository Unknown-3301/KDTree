using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GPUTreeBranch
{
    public int PartitionDimension;
    public float PartitionValue;

    public int BaseBranchIndex;
    public int LowerBranchIndex;
    public int HigherBranchIndex;
    public int Leaf;

    public int ElementsStartIndex;
    public int ElementsCount;

    public static int Size { get => sizeof(int) * 7 + sizeof(float); }
}
