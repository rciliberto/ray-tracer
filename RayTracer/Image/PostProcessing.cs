using RayTracer.Core;

namespace RayTracer.Image;

/// <summary>
///     Utilities to manipulate renders.
/// </summary>
public static class PostProcessing
{
    /// <summary>
    ///     Draws a circle on a render using the
    ///     <a href='https://en.wikipedia.org/wiki/Midpoint_circle_algorithm'>Midpoint Circle Algorithm</a>
    /// </summary>
    /// <param name="x0">Center x value</param>
    /// <param name="y0">Center y value</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="color">Circle color</param>
    /// <param name="render"></param>
    /// <returns></returns>
    public static Render Circle(int x0, int y0, int radius, Color color, Render render)
    {
        var ppm = new Render(render.Width, render.Height, render.MaxColorValue, (render.Raster.Clone() as Color[,])!);

        var f = 1 - radius;
        var ddfX = 1;
        var ddfY = radius * -2;
        var x = 0;
        var y = radius;

        // Draw the initial points based on the radius
        ppm.Raster[x0, y0 + radius] = color;
        ppm.Raster[x0, y0 - radius] = color;
        ppm.Raster[x0 + radius, y0] = color;
        ppm.Raster[x0 - radius, y0] = color;

        while (x < y)
        {
            if (f >= 0)
            {
                y -= 1;
                ddfY += 2;
                f += ddfY;
            }

            x += 1;
            ddfX += 2;
            f += ddfX;

            // Draw the points in all octants
            ppm.Raster[x0 + x, y0 + y] = color;
            ppm.Raster[x0 - x, y0 + y] = color;
            ppm.Raster[x0 + x, y0 - y] = color;
            ppm.Raster[x0 - x, y0 - y] = color;
            ppm.Raster[x0 + y, y0 + x] = color;
            ppm.Raster[x0 - y, y0 + x] = color;
            ppm.Raster[x0 + y, y0 - x] = color;
            ppm.Raster[x0 - y, y0 - x] = color;
        }

        return ppm;
    }

    /// <summary>
    ///     Scale the brightness of a render by a multiplier.
    /// </summary>
    /// <param name="render">The render to scale brightness.</param>
    /// <param name="multiplier">The amount to scale by.</param>
    /// <returns>a new render with brightness scaled.</returns>
    public static Render ScaleLuma(Render render, double multiplier)
    {
        var img = new Render(render.Width, render.Height, render.MaxColorValue, (render.Raster.Clone() as Color[,])!);

        for (var row = 0; row < render.Height; row++)
        for (var col = 0; col < render.Width; col++)
        {
            var color = img.Raster[row, col];

            img.Raster[row, col] = color * multiplier;
        }

        return img;
    }

    /// <summary>
    ///     Gamma correct a render.
    /// </summary>
    /// <param name="render">The render to gamma correct.</param>
    /// <param name="gamma">The gamma value to use.</param>
    /// <returns>a new render with gamma corrected.</returns>
    public static Render GammaCorrect(Render render, float gamma)
    {
        var img = new Render(render.Width, render.Height, render.MaxColorValue, (render.Raster.Clone() as Color[,])!);

        for (var row = 0; row < img.Height; row++)
        for (var col = 0; col < img.Width; col++)
        {
            // Gamma correct each pixel
            var color = img.Raster[row, col];

            var r = Math.Pow(color.R, 1 / gamma);
            var g = Math.Pow(color.G, 1 / gamma);
            var b = Math.Pow(color.B, 1 / gamma);

            img.Raster[row, col] = new Color(r, g, b);
        }

        return img;
    }
}