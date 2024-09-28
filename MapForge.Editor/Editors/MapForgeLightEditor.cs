using MapForge.API.Enums;
using MapForge.API.Spawnables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MapForge.API.Editors
{
    [CustomEditor(typeof(MapForgeLight))]
    public class MapForgeLightEditor : Editor
    {
        private float _lastRange;
        private Color _lastColor;
        private float _lastIntensity;
        private bool _lastShadows;

        public MapForgeLight Base => this.target as MapForgeLight;

        public void OnEnable()
        {
            Base.InitializeInEditor();
            _lastRange = Base.Range;
            _lastColor = Base.Color;
            _lastIntensity = Base.Intensity;
            _lastShadows = Base.Shadows;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Base.Range != _lastRange)
            {
                Base.LightObject.range = Base.Range;
                _lastRange = Base.Range;
            }

            if (Base.Color != _lastColor)
            {
                Base.LightObject.color = Base.Color;
                _lastColor = Base.Color;
            }

            if (Base.Intensity != _lastIntensity)
            {
                Base.LightObject.intensity = Base.Intensity;
                _lastIntensity = Base.Intensity;
            }

            if (Base.Shadows != _lastShadows)
            {
                Base.LightObject.shadows = Base.Shadows ? LightShadows.Soft : LightShadows.None;
                _lastShadows = Base.Shadows;
            }
        }
    }
}
