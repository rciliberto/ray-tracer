using RayTracer.Core;

namespace RayTracer.Material;

/// <summary>
///     Represents a material that can be applied to an object. Used to determine how rays interact with the object.
/// </summary>
public interface IMaterial
{
    /// <summary>
    ///     Whether culling the back faces of the object is allowed for this material. Some transparent materials, such
    ///     as glass, require back faces to be rendered.
    /// </summary>
    public bool CullBackFaces { get; }

    /// <summary>
    ///     Determines how a ray scatters when it hits a material.
    /// </summary>
    /// <param name="ray">The ray that hit the material.</param>
    /// <param name="hitRecord">The hit record of the ray that hit the material.</param>
    /// <param name="attenuation">The attenuation of the scattered ray.</param>
    /// <param name="scattered">The scattered ray.</param>
    /// <returns>true if the ray scatters, false otherwise.</returns>
    public bool Scatter(Ray ray, HitRecord hitRecord, out Color attenuation, out Ray scattered);
}