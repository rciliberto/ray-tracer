using System.Numerics;
using RayTracer.Core;

namespace RayTracer.Material;

/// <summary>
///     Represents a dielectric material. Used for transparent objects.
/// </summary>
public class Dielectric : IMaterial
{
    /// <summary>
    ///     The random number generator used for this material.
    /// </summary>
    private static readonly Random Random = new();

    /// <summary>
    ///     Creates a new dielectric material.
    /// </summary>
    /// <param name="indexOfRefraction">The index of refraction of the dielectric.</param>
    public Dielectric(float indexOfRefraction)
    {
        IndexOfRefraction = indexOfRefraction;
    }

    /// <summary>
    ///     The index of refraction of the dielectric.
    /// </summary>
    public float IndexOfRefraction { get; }

    /// <inheritdoc cref="IMaterial.CullBackFaces" />
    public bool CullBackFaces => true;

    /// <inheritdoc cref="IMaterial.Scatter" />
    public bool Scatter(Ray ray, HitRecord hitRecord, out Color attenuation, out Ray scattered)
    {
        attenuation = new Color(1.0, 1.0, 1.0);
        var refractionRatio = hitRecord.Outward ? 1.0f / IndexOfRefraction : IndexOfRefraction;

        var unitDirection = Vector3.Normalize(ray.Direction);
        var cosTheta = float.Min(Vector3.Dot(-unitDirection, hitRecord.Normal), 1.0f);
        var sinTheta = float.Sqrt(1.0f - cosTheta * cosTheta);

        var direction = refractionRatio * sinTheta > 1.0 ||
                        Reflectance(cosTheta, refractionRatio) > Random.NextSingle()
            ? Vector3Util.Reflect(unitDirection, hitRecord.Normal)
            : Vector3Util.Refract(unitDirection, hitRecord.Normal, refractionRatio);

        scattered = new Ray(hitRecord.HitLocation, direction);

        return true;
    }

    /// <summary>
    ///     Calculates the reflectance of the dielectric. Used to determine if the ray should be reflected or refracted.
    /// </summary>
    /// <param name="cosineTheta">The cosine of the angle between the ray and the normal of the hit record.</param>
    /// <param name="refractiveIndex">The refractive index of the dielectric.</param>
    /// <returns>the reflectance of the dielectric.</returns>
    private static float Reflectance(float cosineTheta, float refractiveIndex)
    {
        // Schlick's approximation for reflectance
        var r0 = (1 - refractiveIndex) / (1 + refractiveIndex);
        r0 *= r0;
        return r0 + (1 - r0) * float.Pow(1 - cosineTheta, 5);
    }
}