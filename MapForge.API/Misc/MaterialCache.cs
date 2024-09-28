using MapForge.API.Enums;
using MapForge.API.Spawnables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapForge.API.Misc
{
    public class MaterialCache
    {
        private static Dictionary<Color, Material> _matCache = new Dictionary<Color, Material>();
        private static Dictionary<Color, List<MapForgePrimitive>> _colorUsage = new Dictionary<Color, List<MapForgePrimitive>>();
        private static Dictionary<SpawnablePrimitiveType, Mesh> _cachedMeshes = new Dictionary<SpawnablePrimitiveType, Mesh>();

        public static void StopUsingColor(MapForgePrimitive metadata)
        {
            if (_colorUsage.ContainsKey(metadata.Color))
                _colorUsage[metadata.Color].Remove(metadata);
        }

        public static Material GetMaterialFromCache(Color color)
        {
            if (_matCache.TryGetValue(color, out Material material))
            {
                if (material == null)
                {
                    material = CreateMaterial(color);
                    _matCache[color] = material;
                }

                return material;
            }

            material = CreateMaterial(color);
            _matCache.Add(color, material);
            return material;
        }

        static Material CreateMaterial(Color color)
        {
            var material = new Material(Shader.Find("Standard"));

            if (color.a < 1f)
            {
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue = 3000;
            }

            material.SetColor("_Color", color);

            return material;
        }

        public static void SetColorUsage(MapForgePrimitive metadata, Color oldColor, Color newColor)
        {
            if (oldColor == newColor)
                return;

            if (_colorUsage.ContainsKey(oldColor))
            {
                _colorUsage[oldColor].Remove(metadata);

                if (_colorUsage[oldColor].Count == 0)
                {
                    _matCache.Remove(oldColor);
                }
            }

            if (_colorUsage.ContainsKey(newColor))
                _colorUsage[newColor].Add(metadata);
            else
                _colorUsage.Add(newColor, new List<MapForgePrimitive>() { metadata });
        }

        public static Mesh GetMeshFromCache(SpawnablePrimitiveType type)
        {
            if (IsMeshCached(type))
                return _cachedMeshes[type];

            Mesh mesh = GetMeshFromPrimitive(type);

            _cachedMeshes.Add(type, mesh);
            return mesh;
        }

        public static bool IsMeshCached(SpawnablePrimitiveType type)
        {
            if (_cachedMeshes.TryGetValue(type, out Mesh mesh))
            {
                if (mesh == null)
                    UpdateCache(type);

                return _cachedMeshes[type];
            }

            return false;
        }

        static Mesh GetMeshFromPrimitive(SpawnablePrimitiveType type)
        {
            GameObject obj = GameObject.CreatePrimitive(type.ToPrimitiveType());

            var filter = obj.GetComponent<MeshFilter>();

            if (filter == null)
                return null;

            Mesh mesh = filter.sharedMesh;

            UnityEngine.Object.DestroyImmediate(obj);

            return mesh;
        }

        static void UpdateCache(SpawnablePrimitiveType type)
        {
            Mesh mesh = GetMeshFromPrimitive(type);

            if (_cachedMeshes.ContainsKey(type))
                _cachedMeshes[type] = mesh;

            else
                _cachedMeshes.Add(type, mesh);
        }
    }
}
