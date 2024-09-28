using MapForge.API.Enums;
using MapForge.API.Misc;
using MapForge.API.Models;
using MapForge.API.Spawnables;
using UnityEngine;

namespace MapForge.API
{
    /// <summary>
    /// Extensions for MapForge.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts MapForge type of spawnable to primitive type.
        /// </summary>
        /// <param name="type">The spawnable primitive type.</param>
        /// <returns>Unity Primitive Type</returns>
        public static PrimitiveType ToPrimitiveType(this SpawnablePrimitiveType type)
        {
            switch (type)
            {
                case SpawnablePrimitiveType.Sphere:
                    return PrimitiveType.Sphere;

                case SpawnablePrimitiveType.Capsule:
                    return PrimitiveType.Capsule;

                case SpawnablePrimitiveType.Cylinder:
                    return PrimitiveType.Cylinder;

                case SpawnablePrimitiveType.Plane:
                    return PrimitiveType.Plane;

                case SpawnablePrimitiveType.Quad:
                    return PrimitiveType.Quad;

                default:
                    return PrimitiveType.Cube;
            }
        }

        public static T GetComponentOrCreate<T>(this Component component) where T : Component
        {
            if (component == null)
                return default;

            if (component.gameObject.TryGetComponent<T>(out T comp))
                return comp;

            return component.gameObject.AddComponent<T>();
        }

        public static void ChangeType(this MapForgePrimitive primitive, SpawnablePrimitiveType primitiveType)
        {
            primitive.PrimitiveType = primitiveType;
            primitive.ObjectFilter.sharedMesh = MaterialCache.GetMeshFromCache(primitiveType);

            UnityEngine.Object.DestroyImmediate(primitive.ObjectCollider);

            if (primitiveType == SpawnablePrimitiveType.Cube)
                primitive.ObjectCollider = primitive.GetComponentOrCreate<BoxCollider>();
            else
            {
                MeshCollider meshCollider = primitive.GetComponentOrCreate<MeshCollider>();
                meshCollider.convex = primitiveType != SpawnablePrimitiveType.Plane && primitiveType != SpawnablePrimitiveType.Quad;

                primitive.ObjectCollider = meshCollider;
            }

            primitive.ObjectCollider.hideFlags = HideFlags.HideInInspector;
        }
    }
}
