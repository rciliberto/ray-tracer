using RayTracer.Core;

namespace RayTracer.Scene.Object.Bounding;

/// <summary>
///     An axis aligned bounding box.
/// </summary>
public class AxisAlignedBoundingBox : IBoundingVolume
{
    /// <summary>
    ///     Constructs a new <see cref="AxisAlignedBoundingBox" />.
    /// </summary>
    /// <param name="minimumPoint">The minimum point of the bounding box.</param>
    /// <param name="maximumPoint">The maximum point of the bounding box.</param>
    public AxisAlignedBoundingBox(Point3 minimumPoint, Point3 maximumPoint)
    {
        MinimumPoint = minimumPoint;
        MaximumPoint = maximumPoint;
    }

    /// <summary>
    ///     The minimum point of the bounding box.
    /// </summary>
    private Point3 MinimumPoint { get; }

    /// <summary>
    ///     The maximum point of the bounding box.
    /// </summary>
    private Point3 MaximumPoint { get; }

    /// <inheritdoc cref="IBoundingVolume.HitBy" />
    public bool HitBy(Ray ray, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        // Check if the ray hits the bounding box.
        for (var i = 0; i < 3; i++)
        {
            var invD = 1.0f / ray.Direction[i];
            var t0 = (MinimumPoint[i] - ray.Origin[i]) * invD;
            var t1 = (MaximumPoint[i] - ray.Origin[i]) * invD;
            if (invD < 0.0f) (t0, t1) = (t1, t0);

            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;
            if (tMax <= tMin) return false;
        }

        return true;
    }
}