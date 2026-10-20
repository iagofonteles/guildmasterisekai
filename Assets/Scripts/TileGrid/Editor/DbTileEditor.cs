using TileGrid.Tiles;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TileGrid.Editor
{
    [CustomEditor(typeof(DbTile), true)]
    public class DbTileEditor : TileEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            AddFields(root, serializedObject);
            return root;
        }

        public static void AddFields(VisualElement root, SerializedObject so)
        {
            var roof = so.FindProperty("roof");
            var id = new PropertyField(so.FindProperty("id"));
            var name = new PropertyField(so.FindProperty("displayName"));
            var desc = new PropertyField(so.FindProperty("description"));
            var icon = new PropertyField(so.FindProperty("icon"));
            var layer = new PropertyField(so.FindProperty("layerMask"));
            var sep = new Label("---") { style = { alignContent = Align.Center } };
            
            root.Insert(0, sep);
            if (roof != null) root.Insert(0, new PropertyField(roof));
            root.Insert(0, layer);
            root.Insert(0, icon);
            root.Insert(0, desc);
            root.Insert(0, name);
            root.Insert(0, id);
        }
    }

    [CustomEditor(typeof(DbAutoTile), true)]
    public class DbAutoTileEditor : AutoTileEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            root.Insert(0, new PropertyField(serializedObject.FindProperty("flags")));
            DbTileEditor.AddFields(root, serializedObject);
            return root;
        }
    }
}