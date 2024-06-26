namespace RayTracer.Core;

/// <summary>
///     Represents RGB Color. Each value is between 0 and 1 where 0 is no intensity and 1 is full intensity.
///     Values may be greater than 1 but not less than 0.
/// </summary>
public readonly partial struct Color
{
    /// <summary>
    ///     The red value of the <see cref="Color" />.
    /// </summary>
    public double R { get; }

    /// <summary>
    ///     The green value of the <see cref="Color" />.
    /// </summary>
    public double G { get; }

    /// <summary>
    ///     The blue value of the <see cref="Color" />.
    /// </summary>
    public double B { get; }

    /// <summary>
    ///     Construct a <see cref="Color" /> with R, G, and B values.
    /// </summary>
    /// <param name="r">The red value.</param>
    /// <param name="g">The green value.</param>
    /// <param name="b">The blue value.</param>
    public Color(double r, double g, double b)
    {
        if (r < 0 || g < 0 || b < 0)
            throw new ArgumentOutOfRangeException(nameof(r), "Color values must be greater than or equal to 0.");

        R = r;
        G = g;
        B = b;
    }

    /// <inheritdoc cref="ValueType.ToString()" />
    public override string ToString()
    {
        return $"{nameof(Color)} [R={R}, G={G}, B={B}]";
    }
}