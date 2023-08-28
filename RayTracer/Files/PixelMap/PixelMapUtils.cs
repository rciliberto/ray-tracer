using System.Text;
using RayTracer.Image;

namespace RayTracer.Files.PixelMap;

/// <summary>
///     Utilities for supporting Reading, Writing and Converting Portable Pixel Map files.
/// </summary>
public static class PixelMapUtils
{
    /// <summary>
    ///     Read a PixelMap file.
    /// </summary>
    /// <param name="fileInfo">The file.</param>
    /// <returns>the <see cref="Render" />.</returns>
    public static Render ReadPixelMap(FileInfo fileInfo)
    {
        using var reader = new StreamReader(fileInfo.OpenRead());
        return reader.ReadPixelMap();
    }

    /// <summary>
    ///     Save the PixelMap to a file.
    /// </summary>
    /// <param name="render">The image.</param>
    /// <param name="fileName">The file.</param>
    /// <param name="pixelMapType">The kind of PPM to write.</param>
    public static void SavePixelMap(this Render render, string fileName, PixelMapType pixelMapType = PixelMapType.P6)
    {
        var fileInfo = new FileInfo(fileName);

        switch (pixelMapType)
        {
            case PixelMapType.P3:
                render.WritePlainPixelMap(fileInfo);
                break;
            case PixelMapType.P6:
                render.WriteRawPixelMap(fileInfo);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pixelMapType), pixelMapType, null);
        }
    }

    /// <summary>
    ///     Writes a plain PixelMap to the file.
    /// </summary>
    /// <param name="render">The image.</param>
    /// <param name="fileInfo">The file.</param>
    private static void WritePlainPixelMap(this Render render, FileInfo fileInfo)
    {
        using var writer = new StreamWriter(fileInfo.OpenWrite());

        // Write meta info
        writer.WriteLine(PixelMapType.P3);
        writer.WriteLine("# CREATOR: Portable Pixel Map Writer by Robby");
        writer.WriteLine($"{render.Width} {render.Height}");
        writer.WriteLine(render.MaxColorValue);

        // Write raster
        for (var row = 0; row < render.Height; row++)
        for (var col = 0; col < render.Width; col++)
        {
            var color = render.Raster[row, col];

            // Truncate max PixelMap value
            var r = (int)(color.R * (render.MaxColorValue - double.Epsilon));
            var g = (int)(color.G * (render.MaxColorValue - double.Epsilon));
            var b = (int)(color.B * (render.MaxColorValue - double.Epsilon));

            writer.WriteLine($"{r} {g} {b}");
        }
    }

    /// <summary>
    ///     Writes a raw PixelMap to the file.
    /// </summary>
    /// <param name="render">The image.</param>
    /// <param name="fileInfo">The file.</param>
    private static void WriteRawPixelMap(this Render render, FileInfo fileInfo)
    {
        using var writer = new BinaryWriter(fileInfo.OpenWrite());

        // Write meta info
        writer.Write(Encoding.UTF8.GetBytes($"{PixelMapType.P6}\n"));
        writer.Write("# CREATOR: Pixel Map Writer by Robby\n"u8.ToArray());
        writer.Write(Encoding.UTF8.GetBytes($"{render.Width} {render.Height}\n"));
        writer.Write(Encoding.UTF8.GetBytes($"{render.MaxColorValue}\n"));

        // Write raster
        for (var row = 0; row < render.Height; row++)
        for (var col = 0; col < render.Width; col++)
        {
            var color = render.Raster[row, col];

            byte[] redBytes;
            byte[] greenBytes;
            byte[] blueBytes;

            // Prepare 1 or 2 byte colors depending on max PixelMap value
            if (render.MaxColorValue > byte.MaxValue)
            {
                var red = (ushort)(color.R * (render.MaxColorValue - double.Epsilon));
                var green = (ushort)(color.G * (render.MaxColorValue - double.Epsilon));
                var blue = (ushort)(color.B * (render.MaxColorValue - double.Epsilon));

                // 2 byte colors
                redBytes = BitConverter.GetBytes(red).Reverse().ToArray();
                greenBytes = BitConverter.GetBytes(green).Reverse().ToArray();
                blueBytes = BitConverter.GetBytes(blue).Reverse().ToArray();
            }
            else
            {
                var red = (byte)(color.R * (render.MaxColorValue - double.Epsilon));
                var green = (byte)(color.G * (render.MaxColorValue - double.Epsilon));
                var blue = (byte)(color.B * (render.MaxColorValue - double.Epsilon));

                // 1 byte colors
                redBytes = new[] { red };
                greenBytes = new[] { green };
                blueBytes = new[] { blue };
            }

            // Write the pixel
            writer.Write(redBytes, 0, redBytes.Length);
            writer.Write(greenBytes, 0, greenBytes.Length);
            writer.Write(blueBytes, 0, blueBytes.Length);
        }
    }
}