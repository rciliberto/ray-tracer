using System.Numerics;
using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object.Bounding;

namespace RayTracer.Scene.Object;

/// <summary>
///     Represents a triangle in a scene. Mostly used for testing.
/// </summary>
public class Triangle : IRenderableObject
{
    /// <summary>
    ///     Creates a new instance of the <see cref="Triangle" /> class.
    /// </summary>
    /// <param name="vertex0">The first vertex of the triangle.</param>
    /// <param name="vertex1">The second vertex of the triangle.</param>
    /// <param name="vertex2">The third vertex of the triangle.</param>
    /// <param name="material">The material of the triangle.</param>
    public Triangle(Point3 vertex0, Point3 vertex1, Point3 vertex2, IMaterial material)
    {
        Vertex0 = vertex0;
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Material = material;
    }

    /// <summary>
    ///     The first vertex of the triangle.
    /// </summary>
    public Point3 Vertex0 { get; set; }

    /// <summary>
    ///     The second vertex of the triangle.
    /// </summary>
    public Point3 Vertex1 { get; set; }

    /// <summary>
    ///     The third vertex of the triangle.
    /// </summary>
    public Point3 Vertex2 { get; set; }

    /// <summary>
    ///     The face normal of the triangle.
    /// </summary>
    public Vector3 Normal => Vector3.Cross(Vertex1 - Vertex0, Vertex2 - Vertex0);

    /// <inheritdoc cref="IRenderableObject.Material" />
    public IMaterial Material { get; set; }

    /// <inheritdoc cref="IRenderableObject.BoundingVolume" />
    public IBoundingVolume? BoundingVolume => null;

    /// <inheritdoc cref="IRenderableObject.Material" />
    public bool HitBy(Ray ray, out HitRecord hitRecord, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        if (ray.IntersectTriangle(Vertex0, Vertex1, Vertex2, out var t, out _, out _, tMin, tMax,
                Material.CullBackFaces))
        {
            hitRecord = new HitRecord(ray, ray.At(t), Normal, Material, t);
            return true;
        }

        hitRecord = new HitRecord();
        return false;
    }
}