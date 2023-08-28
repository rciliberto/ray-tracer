using RayTracer.Core;

namespace RayTracer.Image;

/// <summary>
///     A class for storing a render.
/// </summary>
public class Render
{
    /// <summary>
    ///     The height of the image.
    /// </summary>
    public uint Height;

    /// <summary>
    ///     The maximum color value of the image
    /// </summary>
    public ushort MaxColorValue;

    /// <summary>
    ///     A raster of Height rows, in order from top to bottom. Each row consists of Width pixels, in order from left
    ///     to right. Each pixel is a color value.
    /// </summary>
    public Color[,] Raster;

    /// <summary>
    ///     The width of the image.
    /// </summary>
    public uint Width;

    /// <summary>
    ///     Construct a <see cref="Render" />.
    /// </summary>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    /// <param name="maxColorValue">The maximum color value.</param>
    /// <param name="raster">The raster of pixels.</param>
    public Render(uint width, uint height, ushort maxColorValue, Color[,] raster)
    {
        Width = width;
        Height = height;
        MaxColorValue = maxColorValue;
        Raster = raster;
    }

    /// <summary>
    ///     Construct a <see cref="Render" /> with a black background.
    /// </summary>
    /// <param name="width">The width in pixels</param>
    /// <param name="height">The height in pixels</param>
    /// <param name="maxColorValue">The maximum color value</param>
    public Render(uint width, uint height, ushort maxColorValue) : this(width, height, maxColorValue,
        new Color[height, width])
    {
    }

    /// <summary>
    ///     Create a default 512x512 <see cref="Render" /> with a black background.
    /// </summary>
    public Render() : this(512, 512, 255)
    {
    }
}