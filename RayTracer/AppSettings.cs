namespace RayTracer;

/// <summary>
///     Global application settings.
/// </summary>
public static class AppSettings
{
    /// <summary>
    ///     Whether or not to enable diagnostic statistics.
    /// </summary>
    public static bool EnableDiagnostics { get; set; } = true;

    /// <summary>
    ///     Whether or not to use bounding volumes.
    /// </summary>
    public static bool UseBoundingVolumes { get; set; } = true;

    /// <summary>
    ///     Whether or not to use bounding volumes.
    /// </summary>
    public static bool UseSubMeshBoundingVolumes { get; set; } = true;
}