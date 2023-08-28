using RayTracer.Core;

namespace RayTracer.Material;

/// <summary>
///     Represents a material that has a lambertian reflectance model. Used for matte surfaces.
/// </summary>
public class Lambertian : IMaterial
{
    /// <summary>
    ///     Creates a new lambertian material.
    /// </summary>
    /// <param name="albedo">The albedo of the material.</param>
    /// <param name="cullBackFaces">Whether culling the back faces of the object is allowed for this material.</param>
    public Lambertian(Color albedo, bool cullBackFaces = true)
    {
        Albedo = albedo;
        CullBackFaces = cullBackFaces;
    }

    /// <summary>
    ///     The albedo of the material.
    /// </summary>
    public Color Albedo { get; }

    /// <inheritdoc cref="IMaterial.CullBackFaces" />
    public bool CullBackFaces { get; }

    /// <inheritdoc cref="IMaterial.Scatter" />
    public bool Scatter(Ray ray, HitRecord hitRecord, out Color attenuation, out Ray scattered)
    {
        // Scatter in a random direction.
        var scatterDirection = hitRecord.Normal + Vector3Util.RandomUnitVector();

        // Catch degenerate scatter direction
        if (Vector3Util.NearZero(scatterDirection))
            scatterDirection = hitRecord.Normal;

        scattered = new Ray(hitRecord.HitLocation, scatterDirection);
        attenuation = Albedo;

        // Always scatter
        return true;
    }
}