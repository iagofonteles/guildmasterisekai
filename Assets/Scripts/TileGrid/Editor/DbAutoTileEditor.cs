using GuildMasterIsekai.TileGrid;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Editor
{
    [CustomEditor(typeof(DbAutoTile), true)]
    public sealed class DbAutoTileEditor : AutoTileEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            var item = new PropertyField(serializedObject.FindProperty("item"), "Flags");
            var type = new PropertyField(serializedObject.FindProperty("type"), "Flags");
            var flags = new PropertyField(serializedObject.FindProperty("flags"), "Flags");
            root.Insert(0, flags);
            root.Insert(0, type);
            root.Insert(0, item);
            return root;
        }
    }
}
