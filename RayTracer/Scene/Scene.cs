using RayTracer.Image;
using RayTracer.Scene.Environment;
using RayTracer.Scene.Object;
using RayTracer.Threading;

namespace RayTracer.Scene;

/// <summary>
///     Represents a scene in a 3D environment.
/// </summary>
public class Scene
{
    /// <summary>
    ///     Creates a new scene with the given camera and renderable objects.
    /// </summary>
    /// <param name="camera">The camera of the scene.</param>
    /// <param name="renderableObjects">The renderable objects in the scene.</param>
    /// <param name="environment">The environment of the scene.</param>
    public Scene(Camera camera, RenderableObjectList renderableObjects, IEnvironment environment)
    {
        Camera = camera;
        RenderableObjects = renderableObjects;
        Environment = environment;
    }

    /// <summary>
    ///     Creates a new scene with the default camera and no renderable objects.
    /// </summary>
    public Scene()
    {
        Camera = new Camera();
        RenderableObjects = new RenderableObjectList();
        Environment = new DefaultEnvironment();
    }

    /// <summary>
    ///     The camera of the scene.
    /// </summary>
    public Camera Camera { get; init; }

    /// <summary>
    ///     The renderable objects in the scene.
    /// </summary>
    public RenderableObjectList RenderableObjects { get; init; }

    public IEnvironment Environment { get; set; }

    /// <summary>
    ///     Adds a renderable object to the scene.
    /// </summary>
    /// <param name="renderableObject">The renderable object to add.</param>
    public void AddRenderableObject(IRenderableObject renderableObject)
    {
        RenderableObjects.Add(renderableObject);
    }

    /// <summary>
    ///     Renders the scene. Saves the rendered image to a <see cref="Render" />.
    /// </summary>
    /// <returns>The rendered image.</returns>
    public Render Render(ref float progress)
    {
        return Camera.Render(RenderableObjects, Environment, ref progress);
    }

    /// <summary>
    ///     Renders the scene asynchronously. Saves the rendered image to a <see cref="Render" />.
    /// </summary>
    /// <returns>The rendered image.</returns>
    public Task<Render> RenderAsync(out TaskQueue tasks)
    {
        return Camera.RenderAsync(RenderableObjects, Environment, out tasks);
    }
}