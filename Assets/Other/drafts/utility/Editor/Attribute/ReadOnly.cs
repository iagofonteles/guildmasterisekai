using UnityEngine;
using UnityEditor;
using Drafts;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute), true)]
	public class ReadOnlyPropertyDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var h = 0f;
			property.Reset();
			while(property.NextVisible(true))
				h += EditorGUI.GetPropertyHeight(property);
			return h;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginDisabledGroup(!(attribute as ReadOnlyAttribute).Enable);
			EditorGUI.PropertyField(position, property, label, true);
			EditorGUI.EndDisabledGroup();
			property.serializedObject.ApplyModifiedProperties();
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var prop = new PropertyField(property);
			prop.SetEnabled((attribute as ReadOnlyAttribute).Enable);
			return prop;
		}
	}
}