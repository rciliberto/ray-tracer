using System.Collections.Immutable;
using System.Numerics;
using RayTracer.Core;
using RayTracer.Material;
using RayTracer.Scene.Object.Bounding;

namespace RayTracer.Scene.Object;

/// <summary>
///     Represents a triangle mesh object.
/// </summary>
public class TriangleMesh : IRenderableObject
{
    private readonly OctreeBoundingVolume? _boundingVolume;

    /// <summary>
    ///     Creates a new instance of the <see cref="TriangleMesh" /> class.
    /// </summary>
    /// <param name="faceIndex">The face indices of the mesh.</param>
    /// <param name="vertsIndex">The vertex indices of the mesh.</param>
    /// <param name="verts">The vertices of the mesh.</param>
    /// <param name="vertexNormals">The vertex normals of the mesh.</param>
    /// <param name="vertexNormalIndex">The vertex normal indices of the mesh.</param>
    /// <param name="material">The material of the mesh.</param>
    /// <exception cref="ArgumentException">if the face index or vertex index contains an index that is out of bounds.</exception>
    public TriangleMesh(ICollection<int> faceIndex, ICollection<int> vertsIndex, ICollection<Point3> verts,
        ICollection<Vector3> vertexNormals, ICollection<int> vertexNormalIndex, IMaterial material)
    {
        if (faceIndex.Max() > vertsIndex.Count)
            throw new ArgumentException(
                "Face index contains an index that is out of bounds of the vertex index array.");

        if (vertsIndex.Max() > verts.Count)
            throw new ArgumentException(
                "Vertex index contains an index that is out of bounds of the vertex array.");

        // Find out how many triangles we need to create for this mesh
        Tris = (uint)faceIndex.Sum(face => face - 2);
        P = verts.ToImmutableArray();


        // Create the triangle array triangulated using a simple fan triangulation on the first vertex in the face
        // Assumes convex planar polygons
        var trisIndex = new int[Tris * 3];
        var faceNormals = new List<Vector3>();
        var l = 0;
        for (int i = 0, k = 0; i < faceIndex.Count; i++)
        {
            for (var j = 0; j < faceIndex.ElementAt(i) - 2; j++)
            {
                var v0 = vertsIndex.ElementAt(k);
                var v1 = vertsIndex.ElementAt(k + j + 1);
                var v2 = vertsIndex.ElementAt(k + j + 2);

                var vn0 = vertexNormals.ElementAt(vertexNormalIndex.ElementAt(k));
                var vn1 = vertexNormals.ElementAt(vertexNormalIndex.ElementAt(k + j + 1));
                var vn2 = vertexNormals.ElementAt(vertexNormalIndex.ElementAt(k + j + 2));

                trisIndex[l] = v0;
                trisIndex[l + 1] = v1;
                trisIndex[l + 2] = v2;

                // face normal is the average of the vertex normals
                faceNormals.Add((vn0 + vn1 + vn2) / 3);
                l += 3;
            }

            k += faceIndex.ElementAt(i);
        }

        TrisIndex = trisIndex.ToImmutableArray();
        FaceNormals = faceNormals.ToImmutableArray();
        Material = material;
        TriNumbers = Enumerable.Range(0, (int)Tris).ToImmutableArray();

        // Create the bounding volume
        var xMin = verts.MinBy(v => v.X).X;
        var yMin = verts.MinBy(v => v.Y).Y;
        var zMin = verts.MinBy(v => v.Z).Z;
        var minBound = new Point3(xMin, yMin, zMin);

        var xMax = verts.MaxBy(v => v.X).X;
        var yMax = verts.MaxBy(v => v.Y).Y;
        var zMax = verts.MaxBy(v => v.Z).Z;
        var maxBound = new Point3(xMax, yMax, zMax);

        if (AppSettings.UseBoundingVolumes)
            _boundingVolume = new OctreeBoundingVolume(minBound, maxBound, this,
                TriNumbers);
    }

    public ImmutableArray<int> TriNumbers { get; }

    /// <summary>
    ///     The number of triangles in the mesh.
    /// </summary>
    public uint Tris { get; }

    /// <summary>
    ///     The triangle indices of the mesh.
    /// </summary>
    public ImmutableArray<int> TrisIndex { get; }

    /// <summary>
    ///     The face normals of the mesh.
    /// </summary>
    public ImmutableArray<Vector3> FaceNormals { get; }

    /// <summary>
    ///     The vertices of the mesh.
    /// </summary>
    public ImmutableArray<Point3> P { get; }

    /// <inheritdoc cref="IRenderableObject.Material" />
    public IMaterial Material { get; set; }

    /// <inheritdoc cref="IRenderableObject.BoundingVolume" />
    public IBoundingVolume? BoundingVolume => _boundingVolume;

    /// <inheritdoc cref="IRenderableObject.HitBy" />
    public bool HitBy(Ray ray, out HitRecord hitRecord, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        hitRecord = new HitRecord();

        // Find the nearest triangle that intersects the ray
        var tNear = tMax;
        var nearestTri = -1;

        IEnumerable<int> trisToCheck;

        if (AppSettings.UseBoundingVolumes &&
            AppSettings.UseSubMeshBoundingVolumes &&
            _boundingVolume != null)
        {
            var leafs = _boundingVolume.IntersectingBuds(ray, tMin, tMax);
            trisToCheck = leafs.SelectMany(l => l.TriNumbers);
        }
        else
        {
            trisToCheck = TriNumbers;
        }


        // Loop through all triangles in the mesh
        foreach (var tri in trisToCheck)
        {
            var v0 = P[TrisIndex[tri * 3]];
            var v1 = P[TrisIndex[tri * 3 + 1]];
            var v2 = P[TrisIndex[tri * 3 + 2]];

            // Check if the ray intersects the triangle
            if (ray.IntersectTriangle(v0, v1, v2, out var t, out _, out _, tMin, tMax) && t < tNear)
            {
                // If it does, update the nearest triangle
                tNear = t;
                nearestTri = tri;
                hitRecord = new HitRecord(ray, ray.At(t), FaceNormals[tri], Material, t);
            }
        }

        // Return true if we found a triangle that intersects the ray
        return nearestTri >= 0;
    }
}