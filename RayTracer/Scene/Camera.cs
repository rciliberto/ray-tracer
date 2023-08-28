using System.Numerics;
using RayTracer.Core;
using RayTracer.Diagnostics;
using RayTracer.Image;
using RayTracer.Scene.Environment;
using RayTracer.Scene.Object;
using RayTracer.Threading;

namespace RayTracer.Scene;

/// <summary>
///     Represents a camera in a 3D scene that renders images.
/// </summary>
public class Camera
{
    /// <summary>
    ///     A random number generator for the camera.
    /// </summary>
    private static readonly Random Rand = new();


    /// <summary>
    ///     Construct a camera with the given parameters.
    /// </summary>
    /// <param name="origin">The origin of the camera.</param>
    /// <param name="lookAt">The point the camera is looking at.</param>
    /// <param name="upDirection">The up direction of the camera.</param>
    /// <param name="imageWidth">The width of the image the camera will render in pixels.</param>
    /// <param name="verticalFov">The vertical field of view of the camera.</param>
    /// <param name="aspectRatio">The aspect ratio of the camera.</param>
    /// <param name="aperture">The aperture of the camera.</param>
    /// <param name="samples">The number of samples to take per pixel.</param>
    /// <param name="rayDepth">The maximum number of bounces a ray can take.</param>
    public Camera(Point3 origin, Point3 lookAt, Vector3 upDirection, uint imageWidth, float verticalFov,
        float aspectRatio, float aperture, int samples, int rayDepth)
    {
        Origin = origin;
        LookAt = lookAt;
        UpDirection = upDirection;
        VerticalFov = verticalFov;
        AspectRatio = aspectRatio;
        ImageWidth = imageWidth;
        Samples = samples;
        RayDepth = rayDepth;
        Aperture = aperture;
    }

    /// <summary>
    ///     Construct a camera with default parameters.
    /// </summary>
    public Camera()
    {
        Origin = new Point3(7, 7, -5);
        LookAt = Point3.Zero;
        UpDirection = new Vector3(0, 1, 0);
        VerticalFov = 27;
        AspectRatio = 16f / 9f;
        ImageWidth = 400;
        Samples = 100;
        RayDepth = 50;
        FocusDistance = (Origin - LookAt).Length();
        Aperture = 0;
    }

    /// <summary>
    ///     The origin of the camera.
    /// </summary>
    public Point3 Origin { get; set; }

    /// <summary>
    ///     The point the camera is looking at.
    /// </summary>
    public Point3 LookAt { get; set; }

    /// <summary>
    ///     The up direction of the camera.
    /// </summary>
    public Vector3 UpDirection { get; set; }

    /// <summary>
    ///     The vertical field of view of the camera.
    /// </summary>
    public float VerticalFov { get; set; }

    /// <summary>
    ///     The aspect ratio of the camera.
    /// </summary>
    public float AspectRatio { get; set; }

    /// <summary>
    ///     The aperture of the camera.
    /// </summary>
    public float Aperture { get; set; }

    /// <summary>
    ///     The distance to the focus plane.
    /// </summary>
    public float FocusDistance { get; set; }

    /// <summary>
    ///     The width of the image the camera will render in pixels.
    /// </summary>
    public uint ImageWidth { get; set; }

    /// <summary>
    ///     The height of the image the camera will render in pixels.
    /// </summary>
    public uint ImageHeight
    {
        get => (uint)(ImageWidth / AspectRatio);
        set => ImageWidth = (uint)(value * AspectRatio);
    }

    /// <summary>
    ///     The number of samples to take per pixel.
    /// </summary>
    public int Samples { get; set; }

    /// <summary>
    ///     The maximum number of bounces a ray can take.
    /// </summary>
    public int RayDepth { get; set; }

    /// <summary>
    ///     The vertical field of view in radians.
    /// </summary>
    private float VerticalFovRadians => MathF.PI / 180f * VerticalFov;

    /// <summary>
    ///     The half height of the viewport.
    /// </summary>
    private float HalfHeight => MathF.Tan(VerticalFovRadians / 2f);

    /// <summary>
    ///     The height of the viewport.
    /// </summary>
    private float ViewportHeight => 2f * HalfHeight;

    /// <summary>
    ///     The width of the viewport.
    /// </summary>
    private float ViewportWidth => AspectRatio * ViewportHeight;

    /// <summary>
    ///     The radius of the lens.
    /// </summary>
    private float LensRadius => Aperture / 2f;

    /// <summary>
    ///     The direction the camera is facing.
    /// </summary>
    private Vector3 W => Vector3.Normalize(Origin - LookAt);

    /// <summary>
    ///     The horizontal direction of the camera.
    /// </summary>
    private Vector3 U => Vector3.Normalize(Vector3.Cross(UpDirection, W));

    /// <summary>
    ///     The vertical direction of the camera.
    /// </summary>
    private Vector3 V => Vector3.Cross(W, U);

    /// <summary>
    ///     The horizontal direction of the viewport.
    /// </summary>
    private Vector3 Horizontal => FocusDistance * ViewportWidth * U;

    /// <summary>
    ///     The vertical direction of the viewport.
    /// </summary>
    private Vector3 Vertical => FocusDistance * ViewportHeight * V;

    /// <summary>
    ///     The lower left corner of the viewport.
    /// </summary>
    private Point3 LowerLeftCorner => Origin - Horizontal / 2f - Vertical / 2f - FocusDistance * W;


    /// <summary>
    ///     Render the scene from the camera's point of view.
    /// </summary>
    /// <param name="world">The world to render.</param>
    /// <param name="environment">The environment to render.</param>
    /// <returns>the render of the scene.</returns>
    public Render Render(RenderableObjectList world, IEnvironment environment, ref float progress)
    {
        if (AppSettings.EnableDiagnostics) TraceInfo.StartRender();

        var render = new Render(ImageWidth, ImageHeight, byte.MaxValue);

        // Render the scene from the camera's point of view by iterating over each pixel in the viewport.
        for (var j = (int)render.Height - 1; j >= 0; j--)
        {
            progress = 1 - (float)j / render.Height;
            for (var i = 0; i < render.Width; i++)
                // Sample the viewport at the given coordinates.
                render.Raster[ImageHeight - 1 - j, i] = SampleViewport(i, j, world, environment, Samples);
        }

        if (AppSettings.EnableDiagnostics) TraceInfo.StopRender();

        return render;
    }

    /// <summary>
    ///     Render the scene from the camera's point of view asynchronously.
    /// </summary>
    /// <param name="world">The world to render.</param>
    /// <param name="environment">The environment to render.</param>
    /// <param name="tasks">The tasks that need to be performed before the render completes.</param>
    /// <returns>the render of the scene.</returns>
    public Task<Render> RenderAsync(RenderableObjectList world, IEnvironment environment, out TaskQueue tasks)
    {
        if (AppSettings.EnableDiagnostics) TraceInfo.StartRender();

        var render = new Render(ImageWidth, ImageHeight, byte.MaxValue);

        var taskQueue = new TaskQueue();

        // Render the scene from the camera's point of view by iterating over each pixel in the viewport.
        for (var j = (int)render.Height - 1; j >= 0; j--)
            // Sample the viewport at the given coordinates.
        for (var i = 0; i < render.Width; i++)
        {
            var v = j;
            var u = i;

            taskQueue.Enqueue(_ =>
            {
                render.Raster[ImageHeight - 1 - v, u] = SampleViewport(u, v, world, environment, Samples);
            });
        }

        tasks = taskQueue;
        return Task.Run(() =>
        {
            taskQueue.WaitForCompletion();
            if (AppSettings.EnableDiagnostics) TraceInfo.StopRender();
            return render;
        });
    }

    /// <summary>
    ///     Get the ray that passes through the viewport at the given coordinates.
    /// </summary>
    /// <param name="s">The horizontal coordinate of the viewport.</param>
    /// <param name="t">The vertical coordinate of the viewport.</param>
    /// <returns>the ray that passes through the viewport at the given coordinates.</returns>
    private Ray GetRay(float s, float t)
    {
        var rd = LensRadius * Vector3Util.RandomInUnitDisk();
        var offset = U * rd.X + V * rd.Y;

        return new Ray(
            Origin + offset,
            LowerLeftCorner + s * Horizontal + t * Vertical - Origin - offset
        );
    }

    /// <summary>
    ///     Sample the viewport at the given pixel coordinates a number of times to get the average color.
    /// </summary>
    /// <param name="i">The horizontal coordinate of the viewport.</param>
    /// <param name="j">The vertical coordinate of the viewport.</param>
    /// <param name="world">The list of renderable objects in the scene.</param>
    /// <param name="environment">The environment the scene is in.</param>
    /// <param name="samples">The number of samples to take per pixel.</param>
    /// <returns>the average color of the samples.</returns>
    private Color SampleViewport(float i, float j, RenderableObjectList world, IEnvironment environment, int samples)
    {
        var color = new Color();
        for (var s = 0; s < samples; s++)
        {
            var u = (i + Rand.NextSingle()) / (ImageWidth - 1);
            var v = (j + Rand.NextSingle()) / (ImageHeight - 1);

            color += ColorOf(GetRay(u, v), world, environment, RayDepth);
        }

        return color / samples;
    }

    /// <summary>
    ///     Get the color of a ray cast into the scene.
    /// </summary>
    /// <param name="ray">The ray to cast into the scene.</param>
    /// <param name="world">The list of renderable objects in the scene.</param>
    /// <param name="environment">The environment the scene is in.</param>
    /// <param name="depth">The maximum number of bounces a ray can take.</param>
    /// <returns>the color of the ray cast into the scene.</returns>
    private Color ColorOf(Ray ray, RenderableObjectList world, IEnvironment environment, int depth)
    {
        if (AppSettings.EnableDiagnostics) TraceInfo.RaysCast++;

        var cast = ray;
        var totalAttenuation = new Color(1, 1, 1);

        // Cast the ray into the scene until it hits something or reaches the maximum number of bounces.
        var hits = 0;
        while (hits < depth && world.HitBy(cast, out var rec))
            // If the ray hits something, scatter it and attenuate the color.
            if (rec.Material.Scatter(cast, rec, out var attenuation, out var scattered))
            {
                totalAttenuation *= attenuation;
                cast = scattered;
                hits += 1;

                if (AppSettings.EnableDiagnostics) TraceInfo.RaysCast++;
            }
            // If the ray hits something but cannot be scattered, return black.
            else
            {
                return new Color(0, 0, 0);
            }


        // If the ray reaches the maximum number of bounces, return black.
        if (hits >= depth) return new Color(0, 0, 0);

        // after the ray has bounced around enough and eventually hits nothing (the environment), multiply the
        // attenuation by the environment color.
        return totalAttenuation * environment.EnvironmentColor(ray.Direction);
    }
}