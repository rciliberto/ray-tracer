namespace RayTracer.Core;

/// <summary>
///     Represents a point in 3D space.
/// </summary>
public readonly partial struct Point3
{
    /// <summary>
    ///     The X coordinate of the point.
    /// </summary>
    public float X { get; }

    /// <summary>
    ///     The Y coordinate of the point.
    /// </summary>
    public float Y { get; }

    /// <summary>
    ///     The Z coordinate of the point.
    /// </summary>
    public float Z { get; }

    /// <summary>
    ///     The point at the origin of the coordinate system.
    /// </summary>
    public static Point3 Zero { get; } = new(0, 0, 0);

    /// <summary>
    ///     Construct a point with X, Y, and Z coordinate.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    public Point3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    ///     Returns a value indicating whether this instance and a specified point object represent the same value.
    /// </summary>
    /// <param name="other">A point object to compare to this instance.</param>
    /// <returns>true if other is equal to this instance; otherwise, false.</returns>
    /// <remarks>
    ///     Due to floating point inaccuracies, two vectors are considered equal if the magnitude of the difference is
    ///     less than 1e-5.
    /// </remarks>
    public bool Equals(Point3 other)
    {
        return (this - other).Length() < 1e-5;
    }

    /// <inheritdoc cref="object.Equals(object?)" />
    public override bool Equals(object? obj)
    {
        return obj is Point3 point3 && Equals(point3);
    }

    /// <inheritdoc cref="ValueType.GetHashCode" />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    /// <inheritdoc cref="ValueType.ToString" />
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}