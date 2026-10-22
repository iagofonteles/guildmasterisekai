using TileGrid.Tiles;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TileGrid.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DbAutoTile), true)]
    public class DbAutoTileEditor : AutoTileEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            var so = serializedObject;
            var i = 0;
            
            Insert(root, so, i++, "id");
            Insert(root, so, i++, "displayName");
            Insert(root, so, i++, "description");
            Insert(root, so, i++, "icon");
            Insert(root, so, i++, "roof");
            Insert(root, so, i++, "layerMask");
            Insert(root, so, i++, "flags");
            
            root.Insert(i, new Label("---") { style = { alignContent = Align.Center } });
            return root;
        }

        private static void Insert(VisualElement root, SerializedObject so, int i, string field)
        {
            root.Insert(i, new PropertyField(so.FindProperty(field)));
        }
    }
}