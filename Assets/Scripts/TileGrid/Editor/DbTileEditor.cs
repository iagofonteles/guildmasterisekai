using TileGrid.Tiles;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TileGrid.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DbTile), true)]
    public class DbTileEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var r = new VisualElement();
            var so = serializedObject;

            Add(r, so, "id");
            Add(r, so, "displayName");
            Add(r, so, "description");
            Add(r, so, "icon");
            Add(r, so, "size");
            Add(r, so, "layer");
            Add(r, so, "m_Flags");
            Add(r, so, "m_ColliderType");
            Add(r, so, "m_InstancedGameObject");
            Add(r, so, "rotations");
            Add(r, so, "encodedRotations");

            return r;
        }

        private static void Add(VisualElement root, SerializedObject so, string field)
        {
            root.Add(new PropertyField(so.FindProperty(field)));
        }
    }
}
