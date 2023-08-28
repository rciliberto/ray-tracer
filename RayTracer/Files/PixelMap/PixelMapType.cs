namespace RayTracer.Files.PixelMap;

/// <summary>
///     Represents the type of pixel map, denoted by a magic number at the beginning of a PPM file. It is used to
///     determine if the PPM file is raw or plain.
/// </summary>
public enum PixelMapType
{
    /// <summary>
    ///     Represents plain PPM.
    /// </summary>
    P3,

    /// <summary>
    ///     Represents raw PPM.
    /// </summary>
    P6
}