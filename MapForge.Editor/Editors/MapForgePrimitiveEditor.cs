using MapForge.API.Enums;
using MapForge.API.Misc;
using MapForge.API.Spawnables;
using UnityEditor;
using UnityEngine;

namespace MapForge.API.Editors
{
    [CustomEditor(typeof(MapForgePrimitive))]
    public class MapForgePrimitiveEditor : Editor
    {
        private SpawnablePrimitiveType _lastType;
        private PrimitiveExtraFlags _lastFlags;
        private Color _lastColor;

        public MapForgePrimitive Base => this.target as MapForgePrimitive;

        public void OnEnable()
        {
            Base.InitializeInEditor();
            _lastType = Base.PrimitiveType;
            _lastColor = Base.Color;
            _lastFlags = Base.Flags;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool isVisbile = EditorGUILayout.Toggle("Is Visbile", Base.Flags.HasFlag(PrimitiveExtraFlags.Visible));
            bool canCollide = EditorGUILayout.Toggle("Is Collidable", Base.Flags.HasFlag(PrimitiveExtraFlags.Collidable));

            PrimitiveExtraFlags flags = PrimitiveExtraFlags.None;
            if (isVisbile)
                flags |= PrimitiveExtraFlags.Visible;
            if (canCollide)
                flags |= PrimitiveExtraFlags.Collidable;

            if (flags != Base.Flags)
                Base.Flags = flags;

            if (Base.PrimitiveType != _lastType)
            {
                Base.ChangeType(Base.PrimitiveType);
                _lastType = Base.PrimitiveType;
            }

            if (Base.Flags != _lastFlags)
            {
                if (Base.ObjectCollider != null)
                    Base.ObjectCollider.enabled = Base.Flags.HasFlag(PrimitiveExtraFlags.Collidable);

                if (Base.ObjectRenderer != null)
                    Base.ObjectRenderer.enabled = Base.Flags.HasFlag(PrimitiveExtraFlags.Visible);

                _lastFlags = Base.Flags;
            }

            if (Base.Color != _lastColor)
            {
                Base.ObjectRenderer.sharedMaterial = MaterialCache.GetMaterialFromCache(Base.Color);
                MaterialCache.SetColorUsage(Base, _lastColor, Base.Color);
                _lastColor = Base.Color;
            }
        }
    }
}
