using UnityEngine;
using UnityEditor;
using Drafts;
namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(PrefabAttribute), true)]
	public class PrefabAttributeDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var t = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
			if(!typeof(Component).IsAssignableFrom(t))
				EditorGUI.HelpBox(position, $"Type {t.Name} is not a Component", MessageType.Warning);
			else SearchAttributeDrawer.Draw(position, property, label, GetSettings, (attribute as PrefabAttribute).Type != null);
		}

		ISearchSettings GetSettings() {
			var a = attribute as PrefabAttribute;
			var t = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
			return new PrefabSearchSettings(a.Type ?? t, a.Folder);
		}
	}
}
