using System.Numerics;
using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object.Bounding;

namespace RayTracer.Scene.Object;

/// <summary>
///     Represents a sphere in a 3D scene.
/// </summary>
public class Sphere : IRenderableObject
{
    /// <summary>
    ///     Construct a sphere with a center and radius.
    /// </summary>
    /// <param name="center">The center point of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="material">The material of the sphere.</param>
    public Sphere(Point3 center, float radius, IMaterial material)
    {
        Center = center;
        Radius = radius;
        Material = material;
    }

    /// <summary>
    ///     The center point of the sphere.
    /// </summary>
    public Point3 Center { get; set; }

    /// <summary>
    ///     The radius of the sphere.
    /// </summary>
    public float Radius { get; set; }

    /// <inheritdoc cref="IRenderableObject.Material" />
    public IMaterial Material { get; set; }

    /// <inheritdoc cref="IRenderableObject.BoundingVolume" />
    public IBoundingVolume? BoundingVolume => null;


    /// <inheritdoc cref="IRenderableObject.HitBy" />
    public bool HitBy(Ray ray, out HitRecord hitRecord, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        hitRecord = new HitRecord();

        // Solve the quadratic equation to find the nearest intersection point.

        var oc = ray.Origin - Center;
        var a = ray.Direction.LengthSquared();
        var halfB = Vector3.Dot(oc, ray.Direction);
        var c = oc.LengthSquared() - Radius * Radius;

        var discriminant = halfB * halfB - a * c;

        // If the discriminant is negative, there are no real roots.
        if (discriminant < 0) return false;

        var sqrtD = float.Sqrt(discriminant);

        // Find the nearest root that lies in the acceptable range.
        var root = (-halfB - sqrtD) / a;
        if (root < tMin || tMax < root)
        {
            root = (-halfB + sqrtD) / a;
            if (root < tMin || tMax < root)
                return false;
        }

        // Record the hit.
        var distance = root;
        var hitLocation = ray.At(distance);
        var outwardNormal = (hitLocation - Center) / Radius;

        hitRecord = new HitRecord(ray, hitLocation, outwardNormal, Material, distance);

        return true;
    }
}