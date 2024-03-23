using UnityEditor;
using Drafts.UI.Flow;
using UnityEngine;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(UIAction))]
	public class UIActionInfoDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			=> base.GetPropertyHeight(property, label) * (property.isExpanded ? 2 : 1) +
			(!property.isExpanded ? 0 :
				EditorGUI.GetPropertyHeight(property.FindPropertyRelative("action")) +
				EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onReleased")) +
				EditorGUI.GetPropertyHeight(property.FindPropertyRelative("validation"))
			);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var ctrl = property.FindPropertyRelative("control").stringValue;
			var onPressed = property.FindPropertyRelative("action");
			var onReleased = property.FindPropertyRelative("onReleased");
			var validation = property.FindPropertyRelative("validation");
			var name = property.FindPropertyRelative("name");

			var rect = position;
			rect.height = base.GetPropertyHeight(property, label);
			rect.width /= 2;

			property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, ctrl, true);
			rect.x += rect.width;

			EditorGUI.PropertyField(rect, name, GUIContent.none);
			if(!property.isExpanded) return;

			rect.width = position.width;
			rect.x = position.x;
			rect.y += rect.height;

			rect.height = EditorGUI.GetPropertyHeight(validation);
			EditorGUI.PropertyField(rect, validation);
			rect.y += rect.height;

			rect.height = EditorGUI.GetPropertyHeight(onPressed);
			EditorGUI.PropertyField(rect, onPressed);
			rect.y += rect.height;

			rect.height = EditorGUI.GetPropertyHeight(onReleased);
			EditorGUI.PropertyField(rect, onReleased);
			rect.y += rect.height;
		}
	}
}
