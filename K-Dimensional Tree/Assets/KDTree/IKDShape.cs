using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKDShape
{
    /// <summary>
    /// This is a mark for whether this shape should be deleted. This should only be used by the kdtree.
    /// </summary>
    bool Deleted { get; set; }

    /// <summary>
    /// Returns the <paramref name="dimension"/>-th coordinate of the position of this shape.
    /// </summary>
    /// <param name="dimension">The dimension wanted of the position (0 means x-coordinate, 1 means y-coordinate...)</param>
    /// <returns></returns>
    float GetPosition(int dimension);

    /// <summary>
    /// Returns the <paramref name="dimension"/>-th coordinate of the lowest part of this shape on that same dimension.
    /// <br>For example, if <paramref name="dimension"/> = 0 then this function must return the x position of the left most part of this shape.</br>
    /// </summary>
    /// <param name="dimension">The dimension wanted of the position (0 means x-coordinate, 1 means y-coordinate...)</param>
    /// <returns></returns>
    float GetMin(int dimension);
    /// <summary>
    /// Returns the <paramref name="dimension"/>-th coordinate of the highest part of this shape on that same dimension.
    /// <br>For example, if <paramref name="dimension"/> = 0 then this function must return the x position of the right most part of this shape.</br>
    /// </summary>
    /// <param name="dimension">The dimension wanted of the position (0 means x-coordinate, 1 means y-coordinate...)</param>
    /// <returns></returns>
    float GetMax(int dimension);

    /// <summary>
    /// Returns whether this shape is colliding with an n-dimensional box.
    /// </summary>
    /// <param name="position">The center of the box.</param>
    /// <param name="size">The size of the box.</param>
    /// <returns></returns>
    public bool IsCollidingBox(float[] position, float[] size);

    /// <summary>
    /// Returns whether this shape is colliding with another shape.
    /// </summary>
    /// <param name="other">The other shape.</param>
    /// <returns></returns>
    public bool IsColliding(IKDShape other);
}
