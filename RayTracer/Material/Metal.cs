using System.Numerics;
using RayTracer.Core;

namespace RayTracer.Material;

/// <summary>
///     Represents a metal material. Used for shiny surfaces.
/// </summary>
public class Metal : IMaterial
{
    /// <summary>
    ///     Creates a new metal material.
    /// </summary>
    /// <param name="albedo">The albedo of the material.</param>
    /// <param name="fuzz">The fuzziness of the material.</param>
    /// <param name="cullBackFaces">Whether culling the back faces of the object is allowed for this material.</param>
    public Metal(Color albedo, float fuzz, bool cullBackFaces = true)
    {
        Albedo = albedo;
        Fuzz = fuzz < 1 ? fuzz : 1;
        CullBackFaces = cullBackFaces;
    }

    /// <summary>
    ///     The albedo of the material.
    /// </summary>
    public Color Albedo { get; }

    /// <summary>
    ///     The fuzziness of the material. A value of 0 is a perfect mirror, a value of 1 is a completely diffuse
    ///     surface.
    /// </summary>
    public float Fuzz { get; }

    /// <inheritdoc cref="IMaterial.CullBackFaces" />
    public bool CullBackFaces { get; }

    /// <inheritdoc cref="IMaterial.Scatter" />
    public bool Scatter(Ray ray, HitRecord hitRecord, out Color attenuation, out Ray scattered)
    {
        // Reflect the ray off the surface.
        var reflected = Vector3Util.Reflect(Vector3.Normalize(ray.Direction), hitRecord.Normal);

        // Add fuzziness to the reflected ray.
        scattered = new Ray(hitRecord.HitLocation, reflected + Fuzz * Vector3Util.RandomInUnitSphere());
        attenuation = Albedo;

        // Only scatter if the ray is reflected in the same direction as the normal.
        return Vector3.Dot(scattered.Direction, hitRecord.Normal) > 0;
    }
}