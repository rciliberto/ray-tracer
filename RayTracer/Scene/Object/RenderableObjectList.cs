using RayTracer.Core;
using RayTracer.Diagnostics;

namespace RayTracer.Scene.Object;

/// <summary>
///     Represents a list of renderable objects. Used to render multiple objects.
/// </summary>
public class RenderableObjectList : List<IRenderableObject>
{
    /// <summary>
    ///     Determines if any object in the list of renderable objects is hit by a ray. If it is, it outputs the hit
    ///     record of the closest object.
    /// </summary>
    /// <param name="ray">The ray to check for a hit.</param>
    /// <param name="hitRecord">The hit record of the ray on the closest object.</param>
    /// <param name="tMin">The minimum distance to check for a hit.</param>
    /// <param name="tMax">The maximum distance to check for a hit.</param>
    /// <returns>true if the ray hits an object in the list, false otherwise.</returns>
    public bool HitBy(Ray ray, out HitRecord hitRecord, float tMin = 1e-4f, float tMax = float.MaxValue)
    {
        hitRecord = new HitRecord();

        // Keep track of the distance of the closest object hit so far.
        var closestSoFar = tMax;

        // Check if the ray hits any object in the list.
        var hitAnything = false;

        // Bounding Volume Requirement
        foreach (var renderableObject in this)
        {
            if (AppSettings.UseBoundingVolumes && renderableObject.BoundingVolume != null)
            {
                // If the ray doesn't hit the bounding volume, continue.
                if (!renderableObject.BoundingVolume.HitBy(ray, tMin, closestSoFar)) continue;
                if (AppSettings.EnableDiagnostics) TraceInfo.BoundingVolumeIntersections++;
            }

            // If the ray doesn't hit an object closer than the closest object hit so far, continue.
            if (!renderableObject.HitBy(ray, out var tempRecord, tMin, closestSoFar)) continue;
            if (AppSettings.EnableDiagnostics) TraceInfo.ObjectIntersections++;

            // If the ray hits an object, update the closest object distance.
            closestSoFar = tempRecord.Distance;
            hitRecord = tempRecord;
            hitAnything = true;
        }

        return hitAnything;
    }
}