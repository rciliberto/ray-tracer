using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object.Bounding;

namespace RayTracer.Scene.Object;

/// <summary>
///     Represents an object that can be rendered. Renderable objects can be hit by rays.
/// </summary>
public interface IRenderableObject
{
    /// <summary>
    ///     The material of the object.
    /// </summary>
    public IMaterial Material { get; set; }

    /// <summary>
    ///     The simplified bounding volume of the object. If null, the object is not bounded.
    /// </summary>
    public IBoundingVolume? BoundingVolume { get; }

    /// <summary>
    ///     Determines if the object is hit by a ray. If it is, it outputs the hit record.
    /// </summary>
    /// <param name="ray">The ray to check for a hit.</param>
    /// <param name="hitRecord">The hit record of the ray if it hits the object.</param>
    /// <param name="tMin">The minimum distance to check for a hit.</param>
    /// <param name="tMax">The maximum distance to check for a hit.</param>
    /// <returns>true if the ray hits the object, false otherwise.</returns>
    public bool HitBy(Ray ray, out HitRecord hitRecord, float tMin = 1e-4f, float tMax = float.MaxValue);
}