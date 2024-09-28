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
        private PrimitiveExtraFlags _lastFlags;

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

        [HideInInspector]
        public PrimitiveExtraFlags Flags = PrimitiveExtraFlags.Collidable | PrimitiveExtraFlags.Visible;
        public Action<PrimitiveExtraFlags> FlagsChanged;

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

            if (Flags != _lastFlags)
            {
                if (ObjectRenderer != null)
                    ObjectRenderer.enabled = Flags.HasFlag(PrimitiveExtraFlags.Visible);

                if (ObjectCollider != null)
                    ObjectCollider.enabled = Flags.HasFlag(PrimitiveExtraFlags.Collidable);

                FlagsChanged?.Invoke(Flags);
                _lastFlags = Flags;
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
