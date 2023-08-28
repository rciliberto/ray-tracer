using System.Numerics;

namespace RayTracer.Core;

/// <summary>
///     A collection of utility methods for Vector3.
/// </summary>
public static class Vector3Util
{
    /// <summary>
    ///     A random number generator for the utility methods.
    /// </summary>
    private static readonly Random Rand = new();

    /// <summary>
    ///     Generate a random vector with each component between 0 and 1.
    /// </summary>
    /// <returns>the random vector.</returns>
    public static Vector3 Random()
    {
        return new Vector3(Rand.NextSingle(), Rand.NextSingle(), Rand.NextSingle());
    }

    /// <summary>
    ///     Generate a random vector with each component between some min and max value.
    /// </summary>
    /// <param name="min">The minimum value for each component.</param>
    /// <param name="max">The maximum value for each component.</param>
    /// <returns>the random vector.</returns>
    public static Vector3 Random(float min, float max)
    {
        return Random() * (max - min) + new Vector3(min);
    }

    /// <summary>
    ///     Generate a random vector within the unit sphere.
    /// </summary>
    /// <returns>the random vector.</returns>
    public static Vector3 RandomInUnitSphere()
    {
        Vector3 p;

        // Keep generating random vectors until we find one that is within the unit sphere.
        while ((p = Random(-1, 1)).LengthSquared() >= 1)
        {
        }

        return p;
    }

    /// <summary>
    ///     Generate a normalized random vector within the unit sphere.
    /// </summary>
    /// <returns>the random vector.</returns>
    public static Vector3 RandomUnitVector()
    {
        return Vector3.Normalize(RandomInUnitSphere());
    }

    /// <summary>
    ///     Generate a random vector within the unit hemisphere against a surface normal.
    /// </summary>
    /// <param name="normal">The normal of the surface.</param>
    /// <returns>the random vector.</returns>
    public static Vector3 RandomInHemisphere(Vector3 normal)
    {
        var inUnitSphere = RandomInUnitSphere();

        if (Vector3.Dot(inUnitSphere, normal) > 0.0) // In the same hemisphere as the normal
            return inUnitSphere;

        return -inUnitSphere;
    }

    /// <summary>
    ///     Determine if a vector is near zero in all dimensions.
    /// </summary>
    /// <param name="vector">The vector to check.</param>
    /// <returns>true if the vector is near zero in all dimensions, false otherwise.</returns>
    public static bool NearZero(Vector3 vector)
    {
        const float s = 1e-8f;
        return MathF.Abs(vector.X) < s && MathF.Abs(vector.Y) < s && MathF.Abs(vector.Z) < s;
    }

    /// <summary>
    ///     Calculate the reflection of a vector off a surface. Assumes the vector is normalized.
    /// </summary>
    /// <param name="v">The vector to reflect.</param>
    /// <param name="n">The normal of the surface.</param>
    /// <returns>the reflected vector.</returns>
    public static Vector3 Reflect(Vector3 v, Vector3 n)
    {
        return v - 2 * Vector3.Dot(v, n) * n;
    }

    /// <summary>
    ///     Calculate the refraction of a vector through a surface. Assumes the vector is normalized.
    /// </summary>
    /// <param name="uv">The vector to refract.</param>
    /// <param name="n">The normal of the surface.</param>
    /// <param name="etaIOverEtaT">The ratio of the refractive indices.</param>
    /// <returns>the refracted vector.</returns>
    public static Vector3 Refract(Vector3 uv, Vector3 n, float etaIOverEtaT)
    {
        var cosTheta = float.Min(Vector3.Dot(-uv, n), 1.0f);
        var rOutPerp = etaIOverEtaT * (uv + cosTheta * n);
        var rOutParallel = -float.Sqrt(float.Abs(1.0f - rOutPerp.LengthSquared())) * n;
        return rOutPerp + rOutParallel;
    }

    /// <summary>
    ///     Generate a random vector within the unit disk.
    /// </summary>
    /// <returns>the random vector.</returns>
    public static Vector3 RandomInUnitDisk()
    {
        Vector3 p;
        // Keep generating random vectors until we find one that is within the unit disk.
        while ((p = new Vector3(Rand.NextSingle(), Rand.NextSingle(), 0) * 2 - new Vector3(1, 1, 0))
               .LengthSquared() >= 1)
        {
        }

        return p;
    }
}