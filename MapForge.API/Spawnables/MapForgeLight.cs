using MapForge.API.Enums;
using MapForge.API.Models;
using System;
using UnityEngine;

namespace MapForge.API.Spawnables
{
    public class MapForgeLight : SpawnableInfo
    {
        private float _lastRange;
        private Color _lastColor;
        private float _lastIntensity;
        private bool _lastShadows;

        internal static MapForgeLight Create(SpawnableLightType type, Transform parent)
        {
            GameObject go = new GameObject($"{type} Light");
            go.transform.parent = parent;

            MapForgeLight primitive = go.AddComponent<MapForgeLight>();
            primitive.LightType = type;

            primitive.InitializeInEditor();
            return primitive;
        }

        public override SpawnableType Type { get; } = SpawnableType.Light;

        public SpawnableLightType LightType;

        [Min(0f)]
        public float Range = 10f;
        public Action<float> RangeChanged;

        public Color Color = Color.white;
        public Action<Color> ColorChanged;

        [Min(0f)]
        public float Intensity = 1f;
        public Action<float> IntensityChanged;

        public bool Shadows = true;
        public Action<bool> ShadowsChanged;

        public override void OnUpdate()
        {
            if (Range != _lastRange)
            {
                if (LightObject != null)
                    LightObject.range = Range;

                RangeChanged?.Invoke(Range);
                _lastRange = Range;
            }

            if (Color != _lastColor)
            {
                if (LightObject != null)
                    LightObject.color = Color;

                ColorChanged?.Invoke(Color);
                _lastColor = Color;
            }

            if (Intensity != _lastIntensity)
            {
                if (LightObject != null)
                    LightObject.intensity = Intensity;

                IntensityChanged?.Invoke(Intensity);
                _lastIntensity = Intensity;
            }

            if (Shadows != _lastShadows)
            {
                if (LightObject != null)
                    LightObject.shadows = Shadows ? LightShadows.Soft : LightShadows.None;

                ShadowsChanged?.Invoke(Shadows);
                _lastShadows = Shadows;
            }
        }

        [HideInInspector]
        public Light LightObject;

        internal override void InitializeInEditor()
        {
            LightObject = this.GetComponentOrCreate<Light>();
            LightObject.hideFlags = HideFlags.HideInInspector;

            LightObject.range = Range;
            LightObject.color = Color;
            LightObject.intensity = Intensity;
            LightObject.shadows = Shadows ? LightShadows.Soft : LightShadows.None;

            Debug.Log("Enabled");
        }
    }
}
