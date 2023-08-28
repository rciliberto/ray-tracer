using System.Numerics;

namespace RayTracer.Core;

/// <summary>
///     Represents a ray in a scene with an origin and a direction.
/// </summary>
public readonly struct Ray
{
    /// <summary>
    ///     The origin of the ray. A point that the ray starts from.
    /// </summary>
    public Point3 Origin { get; }

    /// <summary>
    ///     The normalized direction of the ray.
    /// </summary>
    public Vector3 Direction { get; }

    /// <summary>
    ///     Construct a ray with an origin and direction.
    /// </summary>
    /// <param name="origin">The origin point of the ray.</param>
    /// <param name="direction">The direction of the ray.</param>
    public Ray(Point3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = Vector3.Normalize(direction);
    }

    /// <summary>
    ///     Determine the point along the ray that is t units away.
    /// </summary>
    /// <param name="t">The distance along the ray.</param>
    /// <returns>the point that is t units along the ray.</returns>
    public Point3 At(float t)
    {
        return Origin + Direction * t;
    }

    /// <summary>
    ///     Determine if the ray intersects a triangle. Outputs the distance along the ray and the barycentric
    ///     coordinates of the hit.
    /// </summary>
    /// <param name="v0">The first vertex of the triangle.</param>
    /// <param name="v1">The second vertex of the triangle.</param>
    /// <param name="v2">The third vertex of the triangle.</param>
    /// <param name="t">The distance along the ray to the hit.</param>
    /// <param name="u">The u barycentric coordinate of the hit.</param>
    /// <param name="v">The v barycentric coordinate of the hit.</param>
    /// <param name="tMin">The minimum distance along the ray to consider.</param>
    /// <param name="tMax">The maximum distance along the ray to consider.</param>
    /// <param name="cullBackFaces">Whether to cull back faces.</param>
    /// <returns>Whether the ray intersects the triangle.</returns>
    public bool IntersectTriangle(Point3 v0, Point3 v1, Point3 v2, out float t, out float u, out float v,
        float tMin = 1e-4f, float tMax = float.MaxValue, bool cullBackFaces = false)
    {
        t = 0;
        u = 0;
        v = 0;

        // Moller-Trumbore algorithm
        var edgeV0V1 = v1 - v0;
        var edgeV0V2 = v2 - v0;

        var pVec = Vector3.Cross(Direction, edgeV0V2);
        var det = Vector3.Dot(edgeV0V1, pVec);

        // Ray and triangle are parallel if det is close to 0 so no intersection
        if ((cullBackFaces && det < tMin) || (!cullBackFaces && det > -tMin && det < tMin)) return false;

        var invDet = 1f / det;

        // Calculate u barycentric coordinate and test bounds
        var tVec = Origin - v0;
        u = Vector3.Dot(tVec, pVec) * invDet;
        if (u is < 0 or > 1) return false;

        // Calculate v barycentric coordinate and test bounds
        var qVec = Vector3.Cross(tVec, edgeV0V1);
        v = Vector3.Dot(Direction, qVec) * invDet;
        if (v < 0 || u + v > 1) return false;

        // Calculate t to find out where the intersection point is on the line.
        t = Vector3.Dot(edgeV0V2, qVec) * invDet;
        return !(t < tMin) && !(t > tMax);
    }
}