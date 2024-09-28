using MapForge.API.Enums;
using MapForge.API.Misc;
using MapForge.API.Models;
using MapForge.API.Spawnables;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MapForge.API
{
    public static class EditorExtensions
    {
        public static MapForgePrefab GetBundle()
        {
            GameObject target = Selection.activeGameObject;

            if (target == null)
                return null;

            Transform root = target.transform.root;

            if (!root.TryGetComponent(out MapForgePrefab metadata))
                return null;

            return metadata;
        }

        public static void CreateDirectoryFromAssetPath(this string assetPath)
        {
            string directoryPath = Path.GetDirectoryName(assetPath);
            if (Directory.Exists(directoryPath))
                return;
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        public static bool HasAnyChanges(this MapForgePrefab bundle)
        {
            if (!PrefabUtility.IsPartOfPrefabInstance(bundle.gameObject))
                return false;

            PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(bundle.gameObject);

            if (modifications == null || modifications.Length == 0)
                return false;

            return true;
        }

        public static MapForgePrimitive AddPrimitive(this MapForgePrefab bundle, Transform parent, SpawnablePrimitiveType primitiveType)
        {
            MapForgePrimitive primitive = MapForgePrimitive.Create(primitiveType, parent);

            Undo.RegisterCreatedObjectUndo(primitive.gameObject, $"Create primitive {primitiveType}");
            Selection.activeGameObject = primitive.gameObject;

            return primitive;
        }

        public static MapForgeLight AddLight(this MapForgePrefab bundle, Transform parent, SpawnableLightType type)
        {
            MapForgeLight light = MapForgeLight.Create(type, parent);

            Undo.RegisterCreatedObjectUndo(light.gameObject, $"Create light {type}");
            Selection.activeGameObject = light.gameObject;

            return light;
        }
    }
}
