using MapForge.API.Models;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapForge.API
{
    public class MapForgeEditor
    {
        static List<Type> WhitelistedComponents = new List<Type>()
        {
            typeof(Transform),
            typeof(Animator),
            typeof(MapForgePrefab),
            typeof(SpawnableInfo),
        };

        public static void BuildBundle(string bundleName, string outputPath)
        {
            MapForgeLog.Info($"Starting build of bundle <color=orange>{bundleName}</color>...");

            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

            List<string> assetNames = new List<string>();

            for (int x = 0; x < assetPaths.Length; x++)
            {
                string assetPath = assetPaths[x];

                MapForgeLog.Info($"Stripping down prefab <color=orange>{assetPath}</color> <color=orange>{x+1}</color>/<color=orange>{assetPaths.Length}</color>");

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                string tempPath = $"Assets/{prefab.name}.prefab";

                GameObject instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                PrefabUtility.SaveAsPrefabAsset(instanceRoot, tempPath, out bool success);

                UnityEngine.Object.DestroyImmediate(instanceRoot);

                using (var edit = new PrefabUtility.EditPrefabContentsScope(tempPath))
                {
                    foreach (var component in edit.prefabContentsRoot.GetComponentsInChildren<Component>())
                    {
                        Type componentType = component.GetType();

                        if (WhitelistedComponents.Contains(componentType))
                            continue;

                        if (WhitelistedComponents.Contains(componentType.BaseType))
                            continue;

                        MapForgeLog.Info($"Destroy component <color=orange>" + componentType.Name + "</color> on <color=orange>" + component.name + "</color>");
                        UnityEngine.Object.DestroyImmediate(component);
                    }

                    assetNames.Add(tempPath);
                }
            }
            AssetBundleBuild[] build =
            {
                new AssetBundleBuild()
                {
                    assetBundleName = bundleName,
                    assetNames = assetNames.ToArray(),
                }
            };

            BuildPipeline.BuildAssetBundles(outputPath, build, BuildAssetBundleOptions.AssetBundleStripUnityVersion, BuildTarget.StandaloneWindows);

            MapForgeLog.Info($"<color=lime>Build of bundle <color=orange>{bundleName}</color> complete!</color>");

            foreach (var asset in assetNames)
            {
                AssetDatabase.DeleteAsset(asset);
            }
        }
    }
}
