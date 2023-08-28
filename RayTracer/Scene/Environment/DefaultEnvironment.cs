using System.Numerics;
using RayTracer.Core;

namespace RayTracer.Scene.Environment;

/// <summary>
///     Represents the default environment of a scene, a gradient blue sky.
/// </summary>
public class DefaultEnvironment : IEnvironment
{
    /// <inheritdoc cref="IEnvironment.EnvironmentColor" />
    public Color EnvironmentColor(Vector3 direction)
    {
        var unitDirection = Vector3.Normalize(direction);
        var t = 0.5 * (unitDirection.Y + 1.0);
        var c = (1.0 - t) * new Color(1.0, 1.0, 1.0) + t * new Color(0.5, 0.7, 1.0);
        return c;
    }
}