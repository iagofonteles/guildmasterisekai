using UnityEngine;
using UnityEditor;
using Drafts;
using Drafts.Tilemap3D.Generators.Corners;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(RuleConfig))]
	public class RuleConfig3DEditor : PropertyDrawer {

		static GUIContent gcSwap = new GUIContent("S", "swap offset");
		static GUIContent gcConn = new GUIContent("C", "mandatory connections");
		static GUIContent gcFree = new GUIContent("F", "mandatory free spaces");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			var title = label.text.Substring("Element".Length);
			var conn = property.FindPropertyRelative("conn");
			var free = property.FindPropertyRelative("free");
			var swap = property.FindPropertyRelative("swap");

			var r = new ControlRect(position);
			var lw = 12;

			// title / rule / swap 
			EditorGUI.LabelField(r.MoveX(20), title);
			EditorGUI.PropertyField(r.MoveX(-20), swap, GUIContent.none);
			EditorGUI.LabelField(r.MoveX(-lw), gcSwap);

			var w = r.width / 2 - lw;
			EditorGUI.LabelField(r.MoveX(lw), gcConn);
			EditorGUI.PropertyField(r.MoveX(w), conn, GUIContent.none);
			EditorGUI.LabelField(r.MoveX(lw), gcFree);
			EditorGUI.PropertyField(r.NextLine(), free, GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
}