using Drafts;
using DraftsEditor;
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TypeInstance<>), true)]
public class TypeInstanceListDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		=> GetPropertyHeight(property.FindPropertyRelative("list"));

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var comp = (property.GetObject() as ITypeCompatibility)?.Compatibility;
		OnGUI(position, property.FindPropertyRelative("list"), label,
			fieldInfo.FieldType.GetField("list").FieldType.GenericTypeArguments[0],
			TypeSearchSettings.GetCompatibilityFilter(comp));
	}

	public static float GetPropertyHeight(SerializedProperty property) {
		if(!property.isExpanded) return EditorGUIUtility.singleLineHeight;
		return EditorGUI.GetPropertyHeight(property, true) - EditorGUIUtility.singleLineHeight * 2 + 4;
	}

	public static void OnGUI(Rect position, SerializedProperty property, GUIContent label, Type type, Func<Type, bool> filter) {

		void AddElement(Type t) {
			var instance = Activator.CreateInstance(t);
			property.arraySize++;
			var p = property.GetArrayElementAtIndex(property.arraySize - 1);
			p.managedReferenceValue = instance;
			property.serializedObject.ApplyModifiedProperties();
		}

		void DrawHeader(Rect pos) {
			var btnW = 35;
			property.isExpanded = EditorGUI.Foldout(pos.NextX(pos.width - btnW), property.isExpanded, label, true);
			if(GUI.Button(pos, "Add")) {
				var settings = new TypeSearchSettings(type, filter);
				settings.Search(AddElement);
			}
		}

		EditorGUI.BeginProperty(position, label, property);

		if(!property.isExpanded) {
			DrawHeader(position);
			EditorGUI.EndProperty();
			return;
		}

		DrawHeader(position.NextLine());

		EditorGUI.indentLevel++;

		var removeRect = position.GetLine();
		removeRect.width = 60;
		removeRect.x += position.width - removeRect.width;

		EditorGUI.indentLevel++;
		int removeElement = -1;

		for(int i = 0; i < property.arraySize; i++) {
			var ele = property.GetArrayElementAtIndex(i);
			var obj = ele.managedReferenceValue;
			var height = EditorGUI.GetPropertyHeight(ele, true);
			var name = obj?.GetType().Name;
			var tooltip = (obj as IInspectorTooltip)?.Tooltip;
			ele.isExpanded = true;

			if(GUI.Button(removeRect, "Remove")) removeElement = i;
			EditorGUI.PropertyField(position.NextY(height), ele, new(name, tooltip), true);

			removeRect.y += height;
		}
		EditorGUI.indentLevel--;

		if(removeElement >= 0) {
			property.DeleteArrayElementAtIndex(removeElement);
			property.serializedObject.ApplyModifiedProperties();
		}

		property.serializedObject.ApplyModifiedProperties();
		EditorGUI.indentLevel--;
		EditorGUI.EndProperty();
	}
}
