using System.Collections.Immutable;
using RayTracer.Core;
using RayTracer.Diagnostics;

namespace RayTracer.Scene.Object.Bounding;

/// <summary>
///     An octree bounding volume used to partition a mesh.
/// </summary>
public class OctreeBoundingVolume : IBoundingVolume
{
    /// <summary>
    ///     The minimum number of triangles per leaf node.
    /// </summary>
    private const int MinTrisPerNode = 16;

    /// <summary>
    ///     Constructs a new <see cref="OctreeBoundingVolume" />.
    /// </summary>
    /// <param name="minimumPoint">The minimum point of the bounding volume.</param>
    /// <param name="maximumPoint">The maximum point of the bounding volume.</param>
    /// <param name="mesh">The mesh to partition.</param>
    /// <param name="triNumbers">The triangle numbers to include in the bounding volume.</param>
    /// <param name="isRoot">Whether or not this is the root node.</param>
    public OctreeBoundingVolume(Point3 minimumPoint, Point3 maximumPoint, TriangleMesh mesh,
        ImmutableArray<int> triNumbers, bool isRoot = true)
    {
        MinimumPoint = minimumPoint;
        MaximumPoint = maximumPoint;
        TriNumbers = triNumbers;
        Children = new OctreeBoundingVolume[8];
        IsRoot = isRoot;
        IsLeaf = triNumbers.Length <= MinTrisPerNode;


        if (IsLeaf) return;

        var center = new Point3(
            (minimumPoint.X + maximumPoint.X) / 2,
            (minimumPoint.Y + maximumPoint.Y) / 2,
            (minimumPoint.Z + maximumPoint.Z) / 2);

        // Create the children.
        for (var c = 0; c < 8; c++)
        {
            var childMinimumPoint = new Point3(
                c % 2 == 0 ? minimumPoint.X : center.X,
                c % 4 < 2 ? minimumPoint.Y : center.Y,
                c < 4 ? minimumPoint.Z : center.Z);
            var childMaximumPoint = new Point3(
                c % 2 == 0 ? center.X : maximumPoint.X,
                c % 4 < 2 ? center.Y : maximumPoint.Y,
                c < 4 ? center.Z : maximumPoint.Z);

            // Get tris with points in the child.
            var childTris = new List<int>();
            foreach (var tri in triNumbers)
            {
                var v0 = mesh.P[mesh.TrisIndex[tri * 3]];
                var v1 = mesh.P[mesh.TrisIndex[tri * 3 + 1]];
                var v2 = mesh.P[mesh.TrisIndex[tri * 3 + 2]];

                if ((childMinimumPoint <= v0 && v0 <= childMaximumPoint) ||
                    (childMinimumPoint <= v1 && v1 <= childMaximumPoint) ||
                    (childMinimumPoint <= v2 && v2 <= childMaximumPoint))
                {
                    childTris.Add(tri);
                }
            }


            if (childTris.Count > 0)
                Children[c] = new OctreeBoundingVolume(childMinimumPoint, childMaximumPoint, mesh,
                    childTris.ToImmutableArray(), false);

            IsBud = Children.Any(child => child is { IsLeaf: true });
        }
    }

    /// <summary>
    ///     Determines if the bounding volume is a leaf node.
    /// </summary>
    private bool IsLeaf { get; }

    /// <summary>
    ///     Determines if the bounding volume is a bud node. A bud node is a node that has at least one leaf node as a
    ///     child.
    /// </summary>
    private bool IsBud { get; }

    /// <summary>
    ///     If the bounding volume is the root node.
    /// </summary>
    private bool IsRoot { get; }

    /// <summary>
    ///     The children of the bounding volume.
    /// </summary>
    private OctreeBoundingVolume?[] Children { get; }

    /// <summary>
    ///     The triangles in the bounding volume.
    /// </summary>
    internal ImmutableArray<int> TriNumbers { get; }

    /// <summary>
    ///     The minimum point of the bounding volume.
    /// </summary>
    private Point3 MinimumPoint { get; }

    /// <summary>
    ///     The maximum point of the bounding volume.
    /// </summary>
    private Point3 MaximumPoint { get; }

    /// <inheritdoc cref="IBoundingVolume.HitBy" />
    public bool HitBy(Ray ray, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        // Check if the ray hits the bounding box.
        for (var i = 0; i < 3; i++)
        {
            var invD = 1.0f / ray.Direction[i];
            var t0 = (MinimumPoint[i] - ray.Origin[i]) * invD;
            var t1 = (MaximumPoint[i] - ray.Origin[i]) * invD;
            if (invD < 0.0f) (t0, t1) = (t1, t0);

            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;
            if (tMax <= tMin) return false;
        }

        return true;
    }

    /// <summary>
    ///     Get the bounding volumes of all bud nodes in the overall bounding volume that are intersected by the ray.
    /// </summary>
    /// <param name="ray">The ray to check for intersection.</param>
    /// <param name="tMin">The minimum distance to check for intersection.</param>
    /// <param name="tMax">The maximum distance to check for intersection.</param>
    /// <returns>the bounding volumes of all bud nodes that are intersected by the ray.</returns>
    public OctreeBoundingVolume[] IntersectingBuds(Ray ray, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        if (!HitBy(ray, tMin, tMax)) return Array.Empty<OctreeBoundingVolume>();
        TraceInfo.BoundingVolumeIntersections++;
        if ((IsRoot && Children.All(c => c == null)) || IsBud)
            return new[] { this };

        var result = new List<OctreeBoundingVolume>();
        foreach (var child in Children)
        {
            if (child == null) continue;
            result.AddRange(child.IntersectingBuds(ray, tMin, tMax));
        }

        return result.ToArray();
    }
}