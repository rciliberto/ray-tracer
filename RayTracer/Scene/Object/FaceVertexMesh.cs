using System.Collections.Immutable;
using System.Numerics;
using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object.Bounding;

namespace RayTracer.Scene.Object;

/// <summary>
///     Represents a mesh of faces and vertices.
/// </summary>
public class FaceVertexMesh : IRenderableObject
{
    /// <summary>
    ///     The triangle mesh used to render the mesh.
    /// </summary>
    private readonly TriangleMesh _triangleMesh;

    /// <summary>
    ///     Creates a new instance of the <see cref="FaceVertexMesh" /> class.
    /// </summary>
    /// <param name="vertices">The vertices of the mesh.</param>
    /// <param name="vertexTextures">The vertex texture coordinates of the mesh.</param>
    /// <param name="vertexNormals">The vertex normals of the mesh.</param>
    /// <param name="faces">The faces of the mesh.</param>
    /// <param name="material">The material of the mesh.</param>
    public FaceVertexMesh(ICollection<Point3> vertices, ICollection<Vector2> vertexTextures,
        ICollection<Vector3> vertexNormals, ICollection<Face> faces, IMaterial material)
    {
        Vertices = vertices.ToImmutableArray();
        VertexTextures = vertexTextures.ToImmutableArray();
        VertexNormals = vertexNormals.ToImmutableArray();
        Faces = faces.ToImmutableArray();

        var faceIndex = faces.Select(f => f.VertexIndex.Length).ToList();
        var vertexIndex = faces.SelectMany(f => f.VertexIndex).ToList();
        var vertexNormalIndex = faces.SelectMany(f => f.VertexNormalIndex).ToList();
        _triangleMesh = new TriangleMesh(faceIndex, vertexIndex, vertices, vertexNormals, vertexNormalIndex, material);
    }

    /// <summary>
    ///     The vertices of the mesh.
    /// </summary>
    public ImmutableArray<Point3> Vertices { get; }

    /// <summary>
    ///     The vertex texture coordinates of the mesh.
    /// </summary>
    public ImmutableArray<Vector2> VertexTextures { get; }

    /// <summary>
    ///     The vertex normals of the mesh.
    /// </summary>
    public ImmutableArray<Vector3> VertexNormals { get; }

    /// <summary>
    ///     The faces of the mesh.
    /// </summary>
    public ImmutableArray<Face> Faces { get; }

    /// <summary>
    ///     The material of the mesh.
    /// </summary>
    public IMaterial Material
    {
        get => _triangleMesh.Material;
        set => _triangleMesh.Material = value;
    }

    /// <inheritdoc cref="IRenderableObject.BoundingVolume" />
    public IBoundingVolume? BoundingVolume => _triangleMesh.BoundingVolume;

    /// <inheritdoc cref="IRenderableObject.HitBy" />
    public bool HitBy(Ray ray, out HitRecord hitRecord, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        return _triangleMesh.HitBy(ray, out hitRecord, tMin, tMax);
    }

    /// <summary>
    ///     Represents a face of a mesh.
    /// </summary>
    public struct Face
    {
        /// <summary>
        ///     The vertex indices of the face.
        /// </summary>
        public ImmutableArray<int> VertexIndex { get; }

        /// <summary>
        ///     The vertex texture indices of the face.
        /// </summary>
        public ImmutableArray<int> VertexTextureIndex { get; }

        /// <summary>
        ///     The vertex normal indices of the face.
        /// </summary>
        public ImmutableArray<int> VertexNormalIndex { get; }

        /// <summary>
        ///     Creates a new instance of the <see cref="Face" /> struct.
        /// </summary>
        /// <param name="vertexIndex">The vertex indices of the face.</param>
        /// <param name="vertexTextureIndex">The vertex texture indices of the face.</param>
        /// <param name="vertexNormalIndex">The vertex normal indices of the face.</param>
        public Face(IEnumerable<int> vertexIndex, IEnumerable<int> vertexTextureIndex,
            IEnumerable<int> vertexNormalIndex)
        {
            VertexIndex = vertexIndex.ToImmutableArray();
            VertexTextureIndex = vertexTextureIndex.ToImmutableArray();
            VertexNormalIndex = vertexNormalIndex.ToImmutableArray();
        }
    }
}