using UnityEngine;
using UnityEditor;
using Drafts.Patterns;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(SimpleListWatcher<>))]
	public class SimpleListWatcherDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			=> EditorGUI.GetPropertyHeight(property.FindPropertyRelative("list"), label);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			=> EditorGUI.PropertyField(position, property.FindPropertyRelative("list"), label);
	}
}