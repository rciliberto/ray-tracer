using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object;

namespace RayTracer.Files.Wavefront;

/// <summary>
///     Represents a factory for creating objects from Wavefront OBJ files.
/// </summary>
public class WavefrontFactory
{
    /// <summary>
    ///     Creates a new instance of the <see cref="WavefrontFactory" /> class.
    /// </summary>
    /// <param name="defaultMaterial">The default material to use for a generated mesh.</param>
    public WavefrontFactory(IMaterial? defaultMaterial = null)
    {
        DefaultMaterial = defaultMaterial ?? new Lambertian(new Color(0.5, 0.5, 0.5));
    }

    /// <summary>
    ///     The default material to use for a generated mesh.
    /// </summary>
    private IMaterial DefaultMaterial { get; }

    /// <summary>
    ///     Creates a new <see cref="FaceVertexMesh" /> object from the specified Wavefront OBJ file.
    /// </summary>
    /// <param name="objFile">The path to the Wavefront OBJ file.</param>
    /// <returns>a new <see cref="FaceVertexMesh" /> object.</returns>
    public FaceVertexMesh CreateObject(string objFile)
    {
        var objFileInfo = new FileInfo(objFile);
        var fileReader = new StreamReader(objFileInfo.OpenRead());
        return fileReader.ReadObj(DefaultMaterial);
    }

    /// <summary>
    ///     Creates a new <see cref="FaceVertexMesh" /> object from the specified Wavefront OBJ file.
    /// </summary>
    /// <param name="objFile">The Wavefront OBJ file.</param>
    /// <returns>a new <see cref="FaceVertexMesh" /> object.</returns>
    public FaceVertexMesh CreateObject(FileInfo objFile)
    {
        var fileReader = new StreamReader(objFile.OpenRead());
        return fileReader.ReadObj(DefaultMaterial);
    }
}