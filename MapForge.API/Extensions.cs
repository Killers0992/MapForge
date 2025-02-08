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

        /// <summary>
        /// Converts MapForge type of shadow to unity.
        /// </summary>
        /// <param name="type">The shadow type.</param>
        /// <returns>Unity Shadow Type</returns>
        public static LightShadows ToShadowType(this LightShadowType type)
        {
            switch (type)
            {
                case LightShadowType.None:
                    return LightShadows.None;

                case LightShadowType.Soft:
                    return LightShadows.Soft;

                default:
                    return LightShadows.Hard;
            }
        }

        /// <summary>
        /// Converts MapForge type of light shape to unity.
        /// </summary>
        /// <param name="type">The shape type.</param>
        /// <returns>Unity Light Shape</returns>
        public static LightShape ToShapeType(this LightShapeType type)
        {
            switch (type)
            {
                case LightShapeType.Cone:
                    return LightShape.Cone;

                case LightShapeType.Pyramid:
                    return LightShape.Pyramid;

                default:
                    return LightShape.Box;
            }
        }

        /// <summary>
        /// Converts MapForge type of light to unity.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Unity Light Type</returns>
        public static LightType ToLightType(this SpawnableLightType type)
        {
            switch (type)
            {
                case SpawnableLightType.Spot:
                    return LightType.Spot;

                case SpawnableLightType.Directional:
                    return LightType.Directional;

                default:
                    return LightType.Point;
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

            if (primitiveType == SpawnablePrimitiveType.Cube)
            {
                if (primitive.ObjectCollider.GetType() != typeof(BoxCollider))
                    UnityEngine.Object.DestroyImmediate(primitive.ObjectCollider);

                primitive.ObjectCollider = primitive.GetComponentOrCreate<BoxCollider>();
            }
            else
            {
                if (primitive.ObjectCollider.GetType() != typeof(MeshCollider))
                    UnityEngine.Object.DestroyImmediate(primitive.ObjectCollider);

                MeshCollider meshCollider = primitive.GetComponentOrCreate<MeshCollider>();
                meshCollider.convex = primitiveType != SpawnablePrimitiveType.Plane && primitiveType != SpawnablePrimitiveType.Quad;

                primitive.ObjectCollider = meshCollider;
            }

            primitive.ObjectCollider.hideFlags = HideFlags.HideInInspector;
        }
    }
}
