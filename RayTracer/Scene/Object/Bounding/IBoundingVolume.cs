using RayTracer.Core;

namespace RayTracer.Scene.Object.Bounding;

/// <summary>
///     Represents a bounding volume that is used to check if a ray hits an object.
/// </summary>
public interface IBoundingVolume
{
    /// <summary>
    ///     Determines if the bounding volume is hit by a ray. If it is, it outputs the hit record.
    /// </summary>
    /// <param name="ray">The ray to check for a hit.</param>
    /// <param name="t">The distance of the hit location.</param>
    /// <param name="tMin">The minimum distance to check for a hit.</param>
    /// <param name="tMax">The maximum distance to check for a hit.</param>
    /// <returns>true if the ray hits the object, false otherwise.</returns>
    public bool HitBy(Ray ray, float tMin = 1e-4f, float tMax = float.MaxValue);
}