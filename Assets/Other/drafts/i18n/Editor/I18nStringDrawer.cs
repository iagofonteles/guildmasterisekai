using UnityEditor;
using UnityEngine;

namespace Drafts.Internationalization.Editor {

	[CustomPropertyDrawer(typeof(I18nString), true)]
	public class I18nStringDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Rect labelRect, rect;
			labelRect = rect = position;
			labelRect.width = EditorGUIUtility.labelWidth;
			rect.width -= labelRect.width;
			rect.x += labelRect.width;
			rect.width /= 2;

			var table = property.FindPropertyRelative("table");
			var key = property.FindPropertyRelative("key");

			EditorGUI.LabelField(labelRect, label);
			EditorGUI.PropertyField(rect, table, GUIContent.none);

			rect.x += rect.width;
			EditorGUI.PropertyField(rect, key, GUIContent.none);
		}

	}
}
