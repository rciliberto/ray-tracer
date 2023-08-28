using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracer.Core;

// Operators for the Point3D class.
partial struct Point3
{
    public float this[int key] =>
        key switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new IndexOutOfRangeException()
        };

    /// <summary>
    ///     Subtract two points to get a vector in the direction of the first point from the
    ///     second point.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(Point3 a, Point3 b)
    {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    ///     Add a vector to a point to get the point in that direction from the point.
    /// </summary>
    public static Point3 operator +(Point3 a, Vector3 b)
    {
        return new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    /// <summary>
    ///     Subtract a vector to a point to get the point in that direction from the point.
    /// </summary>
    public static Point3 operator -(Point3 a, Vector3 b)
    {
        return new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    ///     Add a point to a vector to get the point in that direction from the point.
    /// </summary>
    public static Point3 operator +(Vector3 a, Point3 b)
    {
        return b + a;
    }

    /// <summary>
    ///     Compare two point are equal.
    /// </summary>
    public static bool operator ==(Point3 a, Point3 b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Compare two point are not equal
    /// </summary>
    public static bool operator !=(Point3 a, Point3 b)
    {
        return !(a == b);
    }

    /// <summary>
    ///     Compare two points less than.
    /// </summary>
    public static bool operator <(Point3 a, Point3 b)
    {
        return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
    }

    /// <summary>
    ///     Compare two points greater than.
    /// </summary>
    public static bool operator >(Point3 a, Point3 b)
    {
        return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
    }

    /// <summary>
    ///     Compare two points less or equal than.
    /// </summary>
    public static bool operator <=(Point3 a, Point3 b)
    {
        return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
    }

    /// <summary>
    ///     Compare two points greater or equal than.
    /// </summary>
    public static bool operator >=(Point3 a, Point3 b)
    {
        return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
    }
}