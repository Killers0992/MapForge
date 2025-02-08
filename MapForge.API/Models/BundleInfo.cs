using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapForge.API.Models
{
    /// <summary>
    /// Information about bundle.
    /// </summary>
    public class BundleInfo
    {
        public BundleInfo(AssetBundle bundle, string path, string name)
        {
            Base = bundle;
            Path = path;
            Name = name;
        }

        /// <summary>
        /// Name of bundle.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Base asset bundle.
        /// </summary>
        public AssetBundle Base { get; private set; }

        /// <summary>
        /// Path of origin location of asset bundle.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// All prefabs inside asset bundle.
        /// </summary>
        public string[] Prefabs => Base.GetAllAssetNames();

        /// <summary>
        /// Gets invoked when bundle is reloaded.
        /// </summary>
        public Action BundleReloaded;

        /// <summary>
        /// A dictionary that stores instances of prefabs, mapping a unique identifier (an integer) 
        /// to its corresponding <see cref="PrefabInfo"/> object.
        /// </summary>
        public Dictionary<int, PrefabInfo> PrefabInstances { get; set; } = new Dictionary<int, PrefabInfo>();

        public PrefabInfo CreatePrefab(string prefabName, int uniqueIdentifier = -1)
        {
            GameObject prefab = Base.LoadAsset<GameObject>(prefabName);

            if (prefab == null)
            {
                // Asset not exists in that bundle.
                return null;
            }

            if (uniqueIdentifier == -1 && PrefabInstances.ContainsKey(uniqueIdentifier))
            {
                // Prefab with that unique identifier is already created.
                return null;
            }

            return new PrefabInfo(GetFreeId(), this, prefabName);
        }

        public bool ContainsPrefab(string prefabName) => Base.Contains(prefabName);

        int GetFreeId()
        {
            for(int x = 0; x < int.MaxValue; x++)
            {
                if (!PrefabInstances.ContainsKey(x))
                    return x;
            }

            // This should never hit 0.
            return 0;
        }

        /// <summary>
        /// Reloads bundle and loads it from file.
        /// </summary>
        public bool Reload()
        {
            foreach(PrefabInfo prefab in PrefabInstances.Values)
            {
                prefab.DeSpawn();
            }
            
            Base?.Unload(true);

            Base = AssetBundle.LoadFromFile(Path);

            if (Base == null)
                return false;

            foreach (PrefabInfo prefab in PrefabInstances.Values.ToArray())
            {
                // If prefab failed to load, example when prefab is removed from bundle after updating file.
                if (!prefab.Spawn())
                    prefab.Unload();
            }

            BundleReloaded?.Invoke();
            return true;
        }

        /// <summary>
        /// Unloads bundle.
        /// </summary>
        public void Unload()
        {
            foreach (PrefabInfo prefab in PrefabInstances.Values.ToArray())
            {
                prefab.Unload();
            }

            if (Base != null)
            {
                Base.Unload(true);
                Base = null;
            }

            MapForgeAPI.LoadedBundles.Remove(Name);
        }
    }
}
