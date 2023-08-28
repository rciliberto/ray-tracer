using System.Numerics;
using RayTracer.Material;

namespace RayTracer.Core;

/// <summary>
///     Represents a hit record of a ray on an object. Contains information about the hit such as the hit location,
///     normal, material, etc.
/// </summary>
public readonly struct HitRecord
{
    /// <summary>
    ///     The location of the hit on the object.
    /// </summary>
    public Point3 HitLocation { get; }

    /// <summary>
    ///     The normal of the hit on the object. Points on the side of the normal that the ray hit.
    /// </summary>
    public Vector3 Normal { get; }

    /// <summary>
    ///     The material of the object that was hit. Used to determine how the ray scatters.
    /// </summary>
    public IMaterial Material { get; }

    /// <summary>
    ///     The distance from the ray origin to the hit location.
    /// </summary>
    public float Distance { get; }

    /// <summary>
    ///     Whether the ray hit the object from the outside or the inside.
    /// </summary>
    public bool Outward { get; }

    /// <summary>
    ///     Creates a new hit record. The normal is calculated based on the ray direction and the outward normal of the
    ///     object.
    /// </summary>
    /// <param name="ray">The ray that hit the object.</param>
    /// <param name="hitLocation">The location of the hit on the object.</param>
    /// <param name="outwardNormal">The outward normal of the object at the hit location.</param>
    /// <param name="material">The material of the object that was hit.</param>
    /// <param name="distance">The distance from the ray origin to the hit location.</param>
    public HitRecord(Ray ray, Point3 hitLocation, Vector3 outwardNormal, IMaterial material, float distance)
    {
        HitLocation = hitLocation;
        Material = material;
        Distance = distance;
        Outward = Vector3.Dot(ray.Direction, outwardNormal) < 0;
        Normal = Outward ? outwardNormal : -outwardNormal;
    }
}