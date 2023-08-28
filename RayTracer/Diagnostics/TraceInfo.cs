using System.Diagnostics;

namespace RayTracer.Diagnostics;

/// <summary>
///     A class to hold diagnostic information about the render.
/// </summary>
public static class TraceInfo
{
    /// <summary>
    ///     A stopwatch to time the render.
    /// </summary>
    public static Stopwatch RenderStopwatch { get; } = new();

    /// <summary>
    ///     The total number of rays cast during the render.
    /// </summary>
    public static int RaysCast { get; set; }

    /// <summary>
    ///     The total number of ray intersections during the render.
    /// </summary>
    public static int ObjectIntersections { get; set; }

    /// <summary>
    ///     The total number of bounding volume intersections during the render.
    /// </summary>
    public static int BoundingVolumeIntersections { get; set; }

    /// <summary>
    ///     Prints the diagnostic information to the console.
    /// </summary>
    public static void PrintDiagnostics()
    {
        Console.WriteLine($"Render Time                   : {RenderStopwatch.Elapsed}");
        Console.WriteLine($"Rays Cast                     : {RaysCast}");
        Console.WriteLine($"Bounding Volume Intersections : {BoundingVolumeIntersections}");
        Console.WriteLine($"Object Intersections          : {ObjectIntersections}");
    }

    /// <summary>
    ///     Resets the diagnostic information.
    /// </summary>
    private static void Reset()
    {
        RenderStopwatch.Reset();
        RaysCast = 0;
        ObjectIntersections = 0;
    }

    /// <summary>
    ///     Resets the diagnostic information and starts the render stopwatch.
    /// </summary>
    public static void StartRender()
    {
        Reset();
        RenderStopwatch.Start();
    }

    /// <summary>
    ///     Stops the render stopwatch.
    /// </summary>
    public static void StopRender()
    {
        RenderStopwatch.Stop();
    }
}