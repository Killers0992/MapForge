using MapForge.API.Enums;
using MapForge.API.Misc;
using MapForge.API.Models;
using System;
using UnityEngine;

namespace MapForge.API.Spawnables
{
    public class MapForgePrimitive : SpawnableInfo
    {
        private SpawnablePrimitiveType _primitiveType;
        private Color _lastColor;
        private bool _lastCollidable, _lastVisibility;

        internal static MapForgePrimitive Create(SpawnablePrimitiveType type, Transform parent)
        {
            GameObject go = new GameObject($"{type} Primitive");
            go.transform.parent = parent;

            MapForgePrimitive primitive = go.AddComponent<MapForgePrimitive>();
            primitive.PrimitiveType = type;

            primitive.InitializeInEditor();
            return primitive;
        }

        public override SpawnableType Type { get; } = SpawnableType.Primitive;

        public SpawnablePrimitiveType PrimitiveType;
        public Action<SpawnablePrimitiveType> PrimitiveTypeChanged;

        public Color Color = Color.white;
        public Action<Color> ColorChanged;

        public bool IsCollidable;
        public Action<bool> CollisionChanged;

        public bool IsVisible;
        public Action<bool> VisibilityChanged;

        public override void OnUpdate()
        {
            if (PrimitiveType != _primitiveType)
            {
                if (ObjectFilter != null)
                    this.ChangeType(PrimitiveType);

                PrimitiveTypeChanged?.Invoke(PrimitiveType);
                _primitiveType = PrimitiveType;
            }

            if (Color != _lastColor)
            {
                if (ObjectRenderer != null)
                {
                    ObjectRenderer.sharedMaterial = MaterialCache.GetMaterialFromCache(Color);
                    MaterialCache.SetColorUsage(this, _lastColor, Color);
                }

                ColorChanged?.Invoke(Color);
                _lastColor = Color;
            }

            if (IsCollidable != _lastCollidable)
            {
                if (ObjectCollider != null)
                    ObjectCollider.enabled = IsCollidable;

                CollisionChanged?.Invoke(IsCollidable);
                _lastCollidable = IsCollidable;
            }

            if (IsVisible != _lastVisibility)
            {
                if (ObjectRenderer != null)
                    ObjectRenderer.enabled = IsVisible;

                VisibilityChanged?.Invoke(IsVisible);
                _lastVisibility = IsVisible;
            }
        }

        [HideInInspector]
        public MeshRenderer ObjectRenderer;

        [HideInInspector]
        public MeshFilter ObjectFilter;

        [HideInInspector]
        public Collider ObjectCollider;

        internal override void InitializeInEditor()
        {
            // Mesh Filter
            ObjectFilter = this.GetComponentOrCreate<MeshFilter>();
            ObjectFilter.hideFlags = HideFlags.HideInInspector;

            ObjectFilter.mesh = MaterialCache.GetMeshFromCache(PrimitiveType);

            // Mesh Renderer
            ObjectRenderer = this.GetComponentOrCreate<MeshRenderer>();
            ObjectRenderer.hideFlags = HideFlags.HideInInspector;

            ObjectRenderer.material = MaterialCache.GetMaterialFromCache(Color);
            MaterialCache.SetColorUsage(this, Color.clear, Color);

            if (PrimitiveType == SpawnablePrimitiveType.Cube)
                ObjectCollider = this.GetComponentOrCreate<BoxCollider>();
            else
            {
                MeshCollider meshCollider = this.GetComponentOrCreate<MeshCollider>();
                meshCollider.convex = PrimitiveType != SpawnablePrimitiveType.Plane && PrimitiveType != SpawnablePrimitiveType.Quad;

                ObjectCollider = meshCollider;
            }

            ObjectCollider.hideFlags = HideFlags.HideInInspector;
        }

        [ExecuteInEditMode]
        private void OnDestroy() => MaterialCache.StopUsingColor(this);
    }
}
