using System.Numerics;
using RayTracer.Core;

namespace RayTracer.Scene.Environment;

/// <summary>
///     Represents the environment of a scene.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    ///     Gets the environment color of a scene based on the direction.
    /// </summary>
    /// <returns>the color of the environment in the direction.</returns>
    public Color EnvironmentColor(Vector3 direction);
}