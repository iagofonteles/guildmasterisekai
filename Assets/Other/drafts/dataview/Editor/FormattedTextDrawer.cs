using Drafts;
using UnityEditor;
using UnityEngine;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(FormattedText))]
	public class FormattedTextDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			var text = property.FindPropertyRelative("text");
			EditorGUI.PropertyField(position, text, label);
			EditorGUI.EndProperty();
		}
	}
}
