using UnityEngine;
using UnityEditor;
using Drafts.Inventory;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(ListInventory<>), true)]
	public class ListInventoryDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			=> EditorGUI.GetPropertyHeight(property.FindPropertyRelative("slots"), label);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			=> EditorGUI.PropertyField(position, property.FindPropertyRelative("slots"), label);
	}
}
