using System;
using UnityEngine;

namespace MapForge.API.Models
{
    public class SpawnableInfo : MonoBehaviour, IMapSpawnable
    {
        public virtual SpawnableType Type { get; }

        public Vector3 Position => transform.position;

        public Vector3 Rotation => transform.eulerAngles;

        public Vector3 Scale => transform.localScale;

        public PrefabInfo SpawnedBy { get; private set; }

        public bool IsAnimated = true;

        public Action UpdatePosition;

        [ExecuteInEditMode]
        private void OnEnable()
        {
            if (!Application.isEditor)
                return;

            InitializeInEditor();
        }

        public void Update()
        {
            if (!IsAnimated)
                return;

            OnUpdate();
            UpdatePosition?.Invoke();
        }

        public virtual void Spawn(PrefabInfo prefab)
        {
            SpawnedBy = prefab;
            MapForgeAPI.Objects.OnSpawnObject(this);
        }

        public virtual void OnUpdate() { }

        internal virtual void InitializeInEditor() { }
    }
}
