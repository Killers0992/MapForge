using MapForge.API.Enums;
using MapForge.API.Models;
using UnityEditor;
using UnityEngine;

namespace MapForge.API.Editors
{
    public static class BaseEditors
    {
        [MenuItem("GameObject/Map Forge/Create Prefab", false, 1)]
        private static void OnCreatePrefab()
        {
            GameObject prefabGo = new GameObject("Prefab");
            MapForgePrefab bundle = prefabGo.AddComponent<MapForgePrefab>();

            string bundlePath = $"Assets/Prefabs/Prefab.prefab";

            bundlePath.CreateDirectoryFromAssetPath();

            PrefabUtility.SaveAsPrefabAssetAndConnect(prefabGo, bundlePath, InteractionMode.AutomatedAction);

            Undo.RegisterCreatedObjectUndo(prefabGo, $"Create prefab");
            Selection.activeTransform = bundle.transform;
        }

        [MenuItem("GameObject/Map Forge/Spawnables/Light", false, 1)]
        private static void OnCreateLight() => EditorExtensions.GetBundle()?.AddLight(Selection.activeTransform, SpawnableLightType.Point);


        [MenuItem("GameObject/Map Forge/Spawnables/Primitives/Quad")]
        private static void OnCreateQuadPrimitive() => EditorExtensions.GetBundle()?.AddPrimitive(Selection.activeTransform, SpawnablePrimitiveType.Quad);

        [MenuItem("GameObject/Map Forge/Spawnables/Primitives/Cube")]
        private static void OnCreateCubePrimitive() => EditorExtensions.GetBundle()?.AddPrimitive(Selection.activeTransform, SpawnablePrimitiveType.Cube);

        [MenuItem("GameObject/Map Forge/Spawnables/Primitives/Plane")]
        private static void OnCreatePlanePrimitive() => EditorExtensions.GetBundle()?.AddPrimitive(Selection.activeTransform, SpawnablePrimitiveType.Plane);

        [MenuItem("GameObject/Map Forge/Spawnables/Primitives/Sphere")]
        private static void OnCreateSpherePrimitive() => EditorExtensions.GetBundle()?.AddPrimitive(Selection.activeTransform, SpawnablePrimitiveType.Sphere);

        [MenuItem("GameObject/Map Forge/Spawnables/Primitives/Capsule")]
        private static void OnCreateCapsulePrimitive() => EditorExtensions.GetBundle()?.AddPrimitive(Selection.activeTransform, SpawnablePrimitiveType.Capsule);

        [MenuItem("GameObject/Map Forge/Spawnables/Primitives/Cylinder")]
        private static void OnCreateCylinderPrimitive() => EditorExtensions.GetBundle()?.AddPrimitive(Selection.activeTransform, SpawnablePrimitiveType.Cylinder);

        [MenuItem("GameObject/Map Forge/Spawnables", true)]
        private static bool OnPrimitiveValidation() => EditorExtensions.GetBundle() != null;
    }
}
