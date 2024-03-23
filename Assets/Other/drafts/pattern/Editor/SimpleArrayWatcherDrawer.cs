using UnityEngine;
using UnityEditor;
using Drafts.Patterns;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(SimpleArrayWatcher<>))]
	public class SimpleArrayWatcherDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			=> EditorGUI.GetPropertyHeight(property.FindPropertyRelative("array"), label);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			=> EditorGUI.PropertyField(position, property.FindPropertyRelative("array"), label);
	}
}
