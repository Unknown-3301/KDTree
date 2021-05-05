using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class KDShape
{
    /// <summary>
    /// The number of dimensions in this shapes.
    /// </summary>
    public int MaxDimensions;
    /// <summary>
    /// Returns the position of the shapes in certain dimension (starting from 0).
    /// <br>EXAMPLE: if this a point with position (5, 10, 6) and this function called with the input of 1 the return must be 10.</br>
    /// </summary>
    /// <param name="dimension">The index of the (position wanted) dimension starting from 0.</param>
    /// <returns></returns>
    public abstract float GetPosition(int dimension);
    /// <summary>
    /// Return the distance from this shape to "another" shape
    /// <br>NOTE: This is seperate function so you can use area shapes distance functions.</br>
    /// </summary>
    /// <param name="another">The other shape to calculate distance from.</param>
    /// <returns></returns>
    public abstract float GetDistance(KDShape another);
    /// <summary>
    /// Returns signed number to know who is bigger in certain dimension.
    /// <br>signed number: Should be either -1, 0 or 1. So -1 mean this shape is smaller than "another" shape</br>
    /// <br>Most of the cases the function would be: Mathf.Sign(GetPosition(dimension) - another.GetPosition(dimension))</br>
    /// </summary>
    /// <param name="another">the other shape to compare.</param>
    /// <param name="dimension">the certain dimension to compate.</param>
    /// <returns></returns>
    public abstract int CompareTo(KDShape another, int dimension);
    /// <summary>
    /// Returns if this shape's position is exactly the same with "another" shape.
    /// </summary>
    /// <param name="another">the other shape to compare position with.</param>
    /// <returns></returns>
    public abstract bool IsEqualPosition(KDShape another);
    /// <summary>
    /// Returns the squared distance from infinite wall.
    /// <br>Wall: infinite wall mean wall that extends in all direction infinitly expet one.</br>
    /// <br>so to calculate the distance get the closest point on that wall to this shape and calculate the distance between</br>
    /// </summary>
    /// <param name="wall">the position of the</param>
    /// <param name="dimension">the dimension that the wall doesnt extend infinity in.</param>
    /// <returns></returns>
    public abstract float DistanceToWall(float wall, int dimension);
}
