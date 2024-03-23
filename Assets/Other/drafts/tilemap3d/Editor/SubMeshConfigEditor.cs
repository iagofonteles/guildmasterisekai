using UnityEngine;
using UnityEditor;
using Drafts;
using Drafts.Tilemap3D.Generators.Corners;

[CustomPropertyDrawer(typeof(SubMeshConfig))]
public class SubMeshConfigEditor : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		var i = label.text.Length - 2;
		var title = label.text.Substring(i < 0 ? 0 : i);
		var use = property.FindPropertyRelative("use");
		var frame = property.FindPropertyRelative("texFrame");
		var mapping = property.FindPropertyRelative("uvMapping");

		var r = new ControlRect(position);

		EditorGUI.LabelField(r.MoveX(12), label);
		EditorGUI.PropertyField(r.MoveX(16), use, GUIContent.none);

		if(!use.boolValue) return;

		EditorGUI.LabelField(r.MoveX(65), "Tex Frame");
		EditorGUI.PropertyField(r.MoveX(24), frame, GUIContent.none);
		EditorGUI.LabelField(r.MoveX(80), "Uv Mapping");
		mapping.enumValueIndex = (int)(UvMapping)EditorGUI.EnumPopup(r.NextLine(), GUIContent.none, (UvMapping)mapping.enumValueIndex);
		//EditorGUI.PropertyField(r.NextLine(), mapping, GUIContent.none);
	}
}
