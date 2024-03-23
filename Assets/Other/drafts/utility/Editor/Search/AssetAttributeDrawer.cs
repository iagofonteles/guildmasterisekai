using UnityEngine;
using UnityEditor;
using Drafts;
namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(AssetAttribute), true)]
	public class AssetAttributeDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(property.propertyType == SerializedPropertyType.String) {
				SearchAttributeDrawer.Draw(position, property, label, GetStringSettings, false);
				return;
			}

			var t = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
			if(!typeof(Object).IsAssignableFrom(t))
				EditorGUI.HelpBox(position, $"Type {t.Name} is not a Unity.Object", MessageType.Warning);
			else SearchAttributeDrawer.Draw(position, property, label, GetAssetSettings, false);
		}

		ISearchSettings GetAssetSettings() {
			var a = attribute as AssetAttribute;
			var t = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
			return new AssetSearchSettings(t, a.Folder);
		}

		ISearchSettings GetStringSettings() {
			var a = attribute as AssetAttribute;
			var t = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
			return new AssetNameSearchSettings(t, a.Folder);
		}
	}
}
