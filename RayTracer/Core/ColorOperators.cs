namespace RayTracer.Core;

// Operators for the <see cref="Color"/> class.
partial struct Color
{
    /// <summary>
    ///     Add two colors together.
    /// </summary>
    public static Color operator +(Color a, Color b)
    {
        return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    /// <summary>
    ///     Multiply a color by some scaler value.
    /// </summary>
    public static Color operator *(Color a, double b)
    {
        return new Color(a.R * b, a.G * b, a.B * b);
    }

    /// <summary>
    ///     Multiply a color by some scaler value.
    /// </summary>
    public static Color operator *(double a, Color b)
    {
        return b * a;
    }

    /// <summary>
    ///     Multiply a color by other color.
    /// </summary>
    public static Color operator *(Color a, Color b)
    {
        return new Color(a.R * b.R, a.G * b.G, a.B * b.B);
    }

    /// <summary>
    ///     Divide a color by some scaler value.
    /// </summary>
    public static Color operator /(Color a, double b)
    {
        return new Color(a.R / b, a.G / b, a.B / b);
    }
}