using UnityEngine;
using UnityEditor;
using Drafts.Inventory;
namespace DraftsEditor {

	[CustomPropertyDrawer(typeof(Slot<>), true)]
	public class SlotDrawer : PropertyDrawer {

		//public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		//	=> EditorGUI.GetPropertyHeight(property.FindPropertyRelative("slots"), label);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var item = property.FindPropertyRelative("item");
			var amount = property.FindPropertyRelative("amount");
			var favorite = property.FindPropertyRelative("favorite");

			if(label.text.StartsWith("Element ")) label.text = label.text.Substring(8);
			var labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 40;

			var rect = position;
			//rect.width = 40;
			//position.width -= rect.width;
			//EditorGUI.LabelField(rect, label);

			//rect.x += rect.width;
			rect.width = position.width - 100;
			position.width -= rect.width;
			EditorGUI.PropertyField(rect, item, label);

			//rect.x += rect.width;
			//rect.width = 20;
			//EditorGUI.LabelField(rect, " #");

			EditorGUIUtility.labelWidth = 20;
			rect.x += rect.width;
			rect.width = 50;
			EditorGUI.PropertyField(rect, amount, new GUIContent(" #"));

			//rect.x += rect.width;
			//rect.width = 30;
			//EditorGUI.LabelField(rect, " Fav");

			EditorGUIUtility.labelWidth = 30;
			rect.x += rect.width;
			rect.width = 50;
			EditorGUI.PropertyField(rect, favorite, new GUIContent(" Fav"));

			EditorGUIUtility.labelWidth = labelWidth;
		}
	}
}
