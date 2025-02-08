using MapForge.API.Spawnables;
using UnityEditor;

namespace MapForge.API.Editors
{
    [CustomEditor(typeof(MapForgePrimitive)), CanEditMultipleObjects]
    public class MapForgePrimitiveEditor : Editor
    {
        public MapForgePrimitive Base => this.target as MapForgePrimitive;

        public void OnEnable() => Base.InitializeInEditor();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Base.OnUpdate();
        }
    }
}
