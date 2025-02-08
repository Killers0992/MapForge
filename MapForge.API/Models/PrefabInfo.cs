using System;
using System.Collections.Generic;
using MapForge.API.Spawnables;
using UnityEngine;

namespace MapForge.API.Models
{
    /// <summary>
    /// Information about prefab.
    /// </summary>
    public class PrefabInfo
    {
        public PrefabInfo(int id, BundleInfo bundle, string prefabName)
        {
            Id = id;
            bundle.PrefabInstances.Add(id, this);

            ParentBundle = bundle;
            PrefabName = prefabName;
        }

        /// <summary>
        /// Prefab unique identifier for this bundle.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Bundle which prefab belongs to.
        /// </summary>
        public BundleInfo ParentBundle { get; }

        /// <summary>
        /// Name of prefab.
        /// </summary>
        public string PrefabName { get; }

        /// <summary>
        /// Instantiated prefab object.
        /// </summary>
        public GameObject Object { get; set; }

        /// <summary>
        /// Checks if prefab is already spawned.
        /// </summary>
        public bool IsSpawned => Object != null;

        /// <summary>
        /// Position of prefab.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotation of prefab.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Scale of prefab.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Event invoked when someone tries to pickup item with has interaction events.
        /// </summary>
        public Action<MapForgePickup, string> OnUsedInteraction;

        /// <summary>
        /// Returns list of spawned sub object from prefab.
        /// </summary>
        public List<GameObject> SubObjects { get; set; } = new List<GameObject>();

        /// <summary>
        /// Gets for which dimension this prefab will be spawned. ( everyone in same dimension will be able to see this )
        /// </summary>
        public int DimensionId { get; set; }

        /// <summary>
        /// Gets spawnables in this prefab.
        /// </summary>
        public SpawnableInfo[] Spawnables { get; set; }

        /// <summary>
        /// Spawns prefab.
        /// </summary>
        /// <param name="position">The position of spawn.</param>
        /// <param name="rotation">The rotatino of spawn.</param>
        /// <param name="scale">The scale of spawn.</param>
        /// <param name="prefabInstance">Provide prefab instance if exists.</param>
        /// <returns></returns>
        public bool Spawn(Vector3 position, Vector3 rotation, Vector3 scale, GameObject prefabInstance = null, int dimensionId = -1)
        {
            // Despawn object if its already spawned.
            if (IsSpawned)
                DeSpawn();

            if (prefabInstance == null)
            {
                prefabInstance = ParentBundle.Base.LoadAsset<GameObject>(PrefabName);
                if (prefabInstance == null)
                {
                    // Something went very wrong.
                    return false;
                }
            }

            DimensionId = dimensionId;

            // Cache for reloading purposes.
            Position = position;
            Rotation = rotation;
            Scale = scale;

            Object = UnityEngine.Object.Instantiate(prefabInstance, Position, Quaternion.Euler(Rotation));
            Object.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Object.transform.localScale = Scale;

            Spawnables = Object.GetComponentsInChildren<SpawnableInfo>();

            foreach (SpawnableInfo spawnable in Spawnables)
            {
                spawnable.Spawn(this);
            }

            return true;
        }

        /// <summary>
        /// Spawns prefab with existing spawn location.
        /// </summary>
        /// <returns>If prefab spawned successfully.</returns>
        public bool Spawn()
        {
            // Despawn object if its already spawned.
            if (IsSpawned)
                DeSpawn();

            return Spawn(Position, Rotation, Scale);
        }

        /// <summary>
        /// Despawns spawned prefab.
        /// </summary>
        /// <returns>If prefab successfully despawned.</returns>
        public bool DeSpawn()
        {
            if (Object == null)
            {
                // Theres nothing to despawn.
                return false;
            }

            foreach(GameObject subObject in SubObjects)
            {
                if (subObject == null)
                    continue;

                UnityEngine.Object.Destroy(subObject);
            }

            SubObjects.Clear();

            UnityEngine.Object.Destroy(Object);
            return true;
        }

        /// <summary>
        /// Unloads prefab.
        /// </summary>
        public void Unload()
        {
            DeSpawn();
            ParentBundle.PrefabInstances.Remove(Id);
        }
    }
}
