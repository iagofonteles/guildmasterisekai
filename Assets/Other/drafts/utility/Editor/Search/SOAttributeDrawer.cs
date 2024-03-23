using UnityEngine;
using UnityEditor;
using Drafts;
namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(SOAttribute), true)]
	public class SOAttributeDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var t = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
			if(!typeof(Object).IsAssignableFrom(t))
				EditorGUI.HelpBox(position, $"Type {t.Name} is not a Unity.Object", MessageType.Warning);
			else SearchAttributeDrawer.Draw(position, property, label, GetSettings, true);
		}

		ISearchSettings GetSettings() {
			var a = attribute as SOAttribute;
			return new SOSearchSettings(a.Type, a.Folder);
		}
	}
}
