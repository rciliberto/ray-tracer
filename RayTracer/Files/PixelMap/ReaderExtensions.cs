using RayTracer.Core;
using RayTracer.Image;

namespace RayTracer.Files.PixelMap;

/// <summary>
///     <see cref="TextReader" /> extensions for reading PPM files.
/// </summary>
internal static class ReaderExtensions
{
    /// <summary>
    ///     Parses a single 'token' in a PPM.
    /// </summary>
    /// <returns>The PPM token.</returns>
    private static string? ReadPpmToken(this TextReader reader)
    {
        // Store characters read so far
        var buffer = new List<char>();

        // As long as we don't hit the end of the file
        while (reader.Peek() >= 0)
        {
            // Read a character
            var c = (char)reader.Read();

            // Skip the line if this marks the beginning of a comment
            if (c == '#' && !buffer.Any())
            {
                reader.ReadLine();
                continue;
            }

            // Stop collecting characters when we hit a whitespace
            if (char.IsWhiteSpace(c)) return new string(buffer.ToArray());

            // Otherwise add the character to the buffer and look again
            buffer.Add(c);
        }

        return buffer.Count > 0 ? new string(buffer.ToArray()) : null;
    }

    /// <summary>
    ///     Reads and parses a PPM file.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>The PPM.</returns>
    /// <exception cref="NotImplementedException">The file supplied is a raw PPM and not plain PPM</exception>
    internal static Render ReadPixelMap(this TextReader reader)
    {
        // Parse magic number
        if (!Enum.TryParse<PixelMapType>(reader.ReadPpmToken(), out var magicNumber))
            throw new InvalidDataException("PPM is invalid: Invalid or missing magic number.");

        // Do not support Raw PPM.
        if (magicNumber == PixelMapType.P6)
            throw new NotImplementedException("Reading raw PPM files are not supported.");

        // Parse width
        if (!uint.TryParse(reader.ReadPpmToken(), out var width))
            throw new InvalidDataException("PPM is invalid: Invalid or missing width.");

        // Parse height
        if (!uint.TryParse(reader.ReadPpmToken(), out var height))
            throw new InvalidDataException("PPM is invalid: Invalid or missing height.");

        // Parse maximum color value
        if (!ushort.TryParse(reader.ReadPpmToken(), out var maxColorValue))
            throw new InvalidDataException("PPM is invalid: Invalid or missing maximum color value.");

        // initialize an empty raster
        var raster = new Color[width, height];

        // Parse P3 raster for rest of file
        for (var row = 0; row < height; row++)
        for (var col = 0; col < width; col++)
        {
            // If the reader hits EOF the default value (0) is used instead.
            ushort.TryParse(reader.ReadPpmToken(), out var red);
            ushort.TryParse(reader.ReadPpmToken(), out var green);
            ushort.TryParse(reader.ReadPpmToken(), out var blue);

            // Add the pixel to the raster
            raster[row, col] = new Color(red, green, blue);
        }

        // Return a new PPM struct with data from the PPM 
        return new Render(width, height, maxColorValue, raster);
    }
}