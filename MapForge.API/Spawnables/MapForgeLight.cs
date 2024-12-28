using MapForge.API.Enums;
using MapForge.API.Models;
using System;
using UnityEngine;

namespace MapForge.API.Spawnables
{
    public class MapForgeLight : SpawnableInfo
    {
        private SpawnableLightType _lastLightType;
        private float _lastRange;
        private Color _lastColor;
        private float _lastIntensity;
        private LightShadowType _lastShadowType;
        private float _lastShadowStrength;
        private LightShapeType _lastShape;
        private float _lastSpotAngle;
        private float _lastInnerSpotAngle;

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
        public Action<SpawnableLightType> LightTypeChanged;

        [Min(0f)]
        public float Range = 10f;
        public Action<float> RangeChanged;

        public Color Color = Color.white;
        public Action<Color> ColorChanged;

        [Min(0f)]
        public float Intensity = 1f;
        public Action<float> IntensityChanged;

        public LightShadowType ShadowType = LightShadowType.None;
        public Action<LightShadowType> ShadowTypeChanged;

        [Range(0f, 1f)]
        public float ShadowStrength = 1f;
        public Action<float> ShadowStrengthChanged;

        public LightShapeType Shape;
        public Action<LightShapeType> ShapeChanged;

        [Range(1f, 179f)]
        public float SpotAngle = 1f;
        public Action<float> SpotAngleChanged;

        [Range(0f, 100f)]
        public float InnerSpotAngle = 1f;
        public Action<float> InnerSpotAngleChanged;

        public void CheckForUpdates<T>(ref T type, ref T lastType, ref Action<T> action, Action changeDetected = null) where T : IComparable
        {
            if (type.CompareTo(lastType) == 0)
                return;

            changeDetected?.Invoke();

            action?.Invoke(type);
            lastType = type;
        }

        public void CheckForUpdates<TS>(ref TS type, ref TS lastType, ref Action<TS> action, Action changeDetected = null, bool b = false) where TS : struct
        {
            if (type.Equals(lastType))
                return;

            changeDetected?.Invoke();

            action?.Invoke(type);
            lastType = type;
        }

        public void CheckForUpdates<TD>(ref TD type, ref TD lastType, ref Action<TD> action, Action changeDetected = null, bool b = false, Enum b2 = null) where TD : IConvertible
        {
            if (type.Equals(lastType))
                return;

            changeDetected?.Invoke();

            action?.Invoke(type);
            lastType = type;
        }

        public override void OnUpdate()
        {
            CheckForUpdates(ref LightType, ref _lastLightType, ref LightTypeChanged, () =>
            {
                if (LightObject != null)
                    LightObject.type = LightType.ToLightType();
            });

            CheckForUpdates(ref Range, ref _lastRange, ref RangeChanged, () =>
            {
                if (LightObject != null)
                    LightObject.range = Range;
            });

            CheckForUpdates(ref Color, ref _lastColor, ref ColorChanged, () =>
            {
                if (LightObject != null)
                    LightObject.color = Color;
            });

            CheckForUpdates(ref Intensity, ref _lastIntensity, ref IntensityChanged, () =>
            {
                if (LightObject != null)
                    LightObject.intensity = Intensity;
            });

            CheckForUpdates(ref ShadowType, ref _lastShadowType, ref ShadowTypeChanged, () =>
            {
                if (LightObject != null)
                    LightObject.shadows = ShadowType.ToShadowType();
            });

            CheckForUpdates(ref ShadowStrength, ref _lastShadowStrength, ref ShadowStrengthChanged, () =>
            {
                if (LightObject != null)
                    LightObject.shadowStrength = ShadowStrength;
            });

            CheckForUpdates(ref Shape, ref _lastShape, ref ShapeChanged, () =>
            {
                if (LightObject != null)
                    LightObject.shape = Shape.ToShapeType();
            });

            CheckForUpdates(ref SpotAngle, ref _lastSpotAngle, ref SpotAngleChanged, () =>
            {
                if (LightObject != null)
                    LightObject.spotAngle = SpotAngle;
            });

            CheckForUpdates(ref InnerSpotAngle, ref _lastInnerSpotAngle, ref InnerSpotAngleChanged, () =>
            {
                if (LightObject != null)
                    LightObject.innerSpotAngle = InnerSpotAngle;
            });
        }

        [HideInInspector]
        public Light LightObject;

        internal override void InitializeInEditor()
        {
            LightObject = this.GetComponentOrCreate<Light>();
            LightObject.hideFlags = HideFlags.HideInInspector;

            LightObject.type = LightType.ToLightType();
            LightObject.range = Range;
            LightObject.color = Color;
            LightObject.intensity = Intensity;
            LightObject.shadows = ShadowType.ToShadowType();
            LightObject.shadowStrength = ShadowStrength;
            LightObject.shape = Shape.ToShapeType();
        }
    }
}
