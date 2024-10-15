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
        private Color _lastColor;
        private bool _lastCollidable, _lastVisibility;

        public MapForgePrimitive Base => this.target as MapForgePrimitive;

        public void OnEnable()
        {
            Base.InitializeInEditor();
            _lastType = Base.PrimitiveType;
            _lastColor = Base.Color;
            _lastCollidable = Base.IsCollidable;
            _lastVisibility = Base.IsVisible;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Base.PrimitiveType != _lastType)
            {
                Base.ChangeType(Base.PrimitiveType);
                _lastType = Base.PrimitiveType;
            }

            if (Base.IsCollidable != _lastCollidable)
            {
                if (Base.ObjectCollider != null)
                    Base.ObjectCollider.enabled = Base.IsCollidable;

                _lastCollidable = Base.IsCollidable;
            }

            if (Base.IsVisible != _lastVisibility)
            {
                if (Base.ObjectRenderer != null)
                    Base.ObjectRenderer.enabled = Base.IsVisible;

                _lastVisibility = Base.IsVisible;
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
