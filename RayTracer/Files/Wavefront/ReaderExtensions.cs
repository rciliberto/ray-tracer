using System.Numerics;
using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object;

namespace RayTracer.Files.Wavefront;

/// <summary>
///     This class is used to extend the TextReader class with a method to read Wavefront .obj files.
/// </summary>
internal static class ReaderExtensions
{
    /// <summary>
    ///     Reads a Wavefront .obj file and returns a FaceVertexMesh object.
    /// </summary>
    /// <param name="reader">The TextReader object to read the file with.</param>
    /// <param name="defaultMaterial">The default material to use for the FaceVertexMesh object.</param>
    /// <returns>A FaceVertexMesh object.</returns>
    /// <exception cref="NotImplementedException">Only triangle meshes are supported.</exception>
    internal static FaceVertexMesh ReadObj(this TextReader reader, IMaterial defaultMaterial)
    {
        var vertices = new List<Point3>();
        var vertexTextures = new List<Vector2>();
        var vertexNormals = new List<Vector3>();
        var faces = new List<FaceVertexMesh.Face>();

        // Read the file line by line.
        while (reader.ReadLine() is { } line)
            // Check if the line is a vertex.
            if (line.StartsWith("v "))
            {
                // Split the line into its components and parse the vertex.
                var vertex = line[2..].Split(' ').Select(float.Parse).ToArray();
                vertices.Add(new Point3(vertex[2], vertex[1], -vertex[0]));
            }
            // Check if the line is a vertex texture.
            else if (line.StartsWith("vt "))
            {
                // Split the line into its components and parse the vertex texture.
                var vertexTexture = line[3..].Split(' ').Select(float.Parse).ToArray();
                vertexTextures.Add(new Vector2(vertexTexture[0], vertexTexture[1]));
            }
            // Check if the line is a vertex normal.
            else if (line.StartsWith("vn "))
            {
                // Split the line into its components and parse the vertex normal.
                var vertexNormal = line[3..].Split(' ').Select(float.Parse).ToArray();
                vertexNormals.Add(new Vector3(vertexNormal[2], vertexNormal[1], -vertexNormal[2]));
            }
            // Check if the line is a face.
            else if (line.StartsWith("f "))
            {
                // Split the line into its components and parse the face.
                var face = line[2..].Split(' ');

                // Ensure that the face is a triangle.
                if (face.Length > 3) throw new NotImplementedException("Only triangles are supported.");

                // Parse the face components.
                var faceVertices = face.Select(x => x.Split('/')).ToArray();
                var vertexIndex = faceVertices.Select(x => int.Parse(x[0]) - 1).ToArray();
                var vertexTextureIndex = faceVertices.Where(x => x[1] != "").Select(x => int.Parse(x[1]) - 1).ToArray();
                var vertexNormalIndex = faceVertices.Select(x => int.Parse(x[2]) - 1).ToArray();

                // Add the face to the mesh.
                faces.Add(new FaceVertexMesh.Face(vertexIndex, vertexTextureIndex, vertexNormalIndex));
            }

        return new FaceVertexMesh(vertices, vertexTextures, vertexNormals, faces, defaultMaterial);
    }
}