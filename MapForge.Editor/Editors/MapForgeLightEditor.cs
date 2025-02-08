using MapForge.API.Spawnables;
using UnityEditor;

namespace MapForge.API.Editors
{
    [CustomEditor(typeof(MapForgeLight)), CanEditMultipleObjects]
    public class MapForgeLightEditor : Editor
    {
        public MapForgeLight Base => this.target as MapForgeLight;

        public void OnEnable() => Base.InitializeInEditor();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Base.OnUpdate();
        }
    }
}
