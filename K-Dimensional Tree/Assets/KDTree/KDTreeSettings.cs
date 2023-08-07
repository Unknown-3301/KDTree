using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KDTreeSettings
{
    /// <summary>
    /// The maximum number of elements allowed in a single leaf node. Recommended 10.
    /// </summary>
    public int MaxLeafSize;
    /// <summary>
    /// The minimum percentage different between two related branches to rebuild them to restore the trees balance. Recommended 0.5
    /// </summary>
    public float RebuildPercent;
    /// <summary>
    /// If true, the elements will only be stored in the leaf branches, however performace might decrease. Recommended false. The theoretical memory usage depending on this variable, where N is the number of elements is as follows:
    /// <br>If true: N.</br>
    /// <br>If false: N * ceil(Log2(N / <see cref="MaxLeafSize"/>)).</br>
    /// </summary>
    public bool StoreElementsInLeafsOnly;
}
