using MapForge.API.Models;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MapForge.API.Editors
{
    [CustomEditor(typeof(MapForgePrefab))]
    public class MapForgePrefabEditor : Editor
    {
        static string _buildOutput = string.Empty;

        private string[] allAssetBundleNames;
        private int selectedBundleIndex = 0;
        private bool isCreatingNewBundle = false;
        private string newBundleName = "";
        private string currentBundleName = "";
        private Vector2 _prefabsScroll = Vector2.zero;

        public MapForgePrefab Base => this.target as MapForgePrefab;
        
        public GameObject Prefab
        {
            get
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(Base.gameObject);

                if (prefab != null)
                    return prefab;

                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage != null)
                    return prefabStage.prefabContentsRoot;

                return null;
            }
        }

        public string AssetPath
        {
            get
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(Base.gameObject);

                if (prefab != null)
                    return AssetDatabase.GetAssetPath(prefab);

                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage != null)
                    return prefabStage.assetPath;

                return string.Empty;
            }
        }

        public bool HasUnsavedChanges
        {
            get
            {
                PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(Base.gameObject);

                if (modifications != null && modifications.Length > 0)
                    return true;

                return false;
            }
        }

        void OnEnable()
        {
            RefreshAssetBundleList();

            foreach(SpawnableInfo info in Base.GetComponentsInChildren<SpawnableInfo>(true))
            {
                info.InitializeInEditor();
            }
        }

        private void RefreshAssetBundleList()
        {
            allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();

            ArrayUtility.Insert(ref allAssetBundleNames, 0, "None");
            ArrayUtility.Add(ref allAssetBundleNames, "Create New...");

            AssetImporter importer = AssetImporter.GetAtPath(AssetPath);

            if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
            {
                currentBundleName = importer.assetBundleName;
                selectedBundleIndex = System.Array.IndexOf(allAssetBundleNames, currentBundleName);
            }
            else
            {
                currentBundleName = "";
                selectedBundleIndex = 0;
            }

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(AssetPath);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15f);

            bool isValid = true;
            bool canBuild = true;

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Validation checks");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label("Is prefab?");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Prefab == null ? "No" : "Yes");

            if (Prefab == null)
                isValid = false;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label("Is output path valid?");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Directory.Exists(_buildOutput) ? "Yes" : "No");

            if (!Directory.Exists(_buildOutput))
                canBuild = false;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (!isValid)
                canBuild = false;

            GUILayout.Space(15f);

            GUI.enabled = isValid;

            selectedBundleIndex = EditorGUILayout.Popup("Select bundle", selectedBundleIndex, allAssetBundleNames);

            if (selectedBundleIndex == 0 && !string.IsNullOrEmpty(currentBundleName))
            {
                AssignAssetBundle(string.Empty);
            }
            else if (selectedBundleIndex == allAssetBundleNames.Length - 1)
            {
                isCreatingNewBundle = true;
                EditorGUILayout.Space();
                newBundleName = EditorGUILayout.TextField("New AssetBundle Name", newBundleName);

                if (!string.IsNullOrEmpty(newBundleName) && GUILayout.Button("Create and Assign New AssetBundle"))
                {
                    AssignAssetBundle(newBundleName);
                    isCreatingNewBundle = false;
                }
            }
            else if (selectedBundleIndex != 0)
            {
                AssignAssetBundle(allAssetBundleNames[selectedBundleIndex]);
            }

            if (!isValid)
                return;

            GUILayout.Space(50f);

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Build settings");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Output path");
            _buildOutput = GUILayout.TextField(_buildOutput);
            if (GUILayout.Button("Select"))
            {
                _buildOutput = EditorUtility.OpenFolderPanel("Select output path", "", "");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Prefabs");
            _prefabsScroll = GUILayout.BeginScrollView(_prefabsScroll, false, true, GUILayout.MinHeight(100f));
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(currentBundleName);
            foreach (var prefab in assetPaths)
            {
                GameObject asset = (GameObject)AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject));

                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.ObjectField(asset, typeof(GameObject), false, GUILayout.MaxWidth(90f));
                GUILayout.Label(prefab);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (canBuild)
            {
                EditorGUILayout.HelpBox("All prefabs will be build together if they are in same bundle", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Select output path!", MessageType.Error);
            }

            GUI.enabled = canBuild;

            if (GUILayout.Button("Build"))
            {
                if (HasUnsavedChanges)
                {
                    if (!EditorUtility.DisplayDialog("Prefab contains unsaved changes", "Do you want to apply unsaved changes onto prefab?", "Yes", "Cancel"))
                        return;
                    else
                    {
                        PrefabUtility.ApplyPrefabInstance(Base.gameObject, InteractionMode.UserAction);
                    }
                }

                MapForgeEditor.BuildBundle(currentBundleName, _buildOutput);
            }

            GUI.enabled = true;
        }

        private void AssignAssetBundle(string bundleName)
        {
            bundleName = bundleName.ToLower();

            if (string.IsNullOrEmpty(AssetPath))
            {
                Debug.LogError("Selected object is not a valid asset.");
                return;
            }

            AssetImporter importer = AssetImporter.GetAtPath(AssetPath);
            if (importer != null)
            {
                importer.assetBundleName = bundleName;
                AssetDatabase.SaveAssets();

                RefreshAssetBundleList();
            }
            else
            {
                Debug.LogError("Could not find AssetImporter for the selected asset.");
            }
        }
    }
}
