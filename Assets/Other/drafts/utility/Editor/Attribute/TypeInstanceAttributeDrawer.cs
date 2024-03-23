using UnityEngine;
using UnityEditor;
using Drafts;
using System;
using DraftsEditor;

[CustomPropertyDrawer(typeof(TypeInstanceAttribute))]
public class TypeInstanceAttributeDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		=> GetPropertyHeight(property);

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		=> OnGUI(position, property, label, fieldInfo.FieldType);

	public static float GetPropertyHeight(SerializedProperty property) {
		if(!property.isExpanded) return EditorGUIUtility.singleLineHeight;
		return Mathf.Max(EditorGUI.GetPropertyHeight(property, true), EditorGUIUtility.singleLineHeight);
	}

	public static void OnGUI(Rect position, SerializedProperty property, GUIContent label, Type fieldType) {
		if(property.propertyType != SerializedPropertyType.ManagedReference)
			throw new Exception("Field is not a ManagedReference");

		EditorGUI.BeginProperty(position, label, property);
		var currType = property.managedReferenceValue?.GetType();

		var rect = position;
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.width /= 2;
		rect.x += rect.width;
		DrawButton(rect, property, new(currType?.Name), fieldType);

		if(currType == null) EditorGUI.LabelField(position, label);
		else EditorGUI.PropertyField(position, property, label, true);

		EditorGUI.EndProperty();
	}

	static ISearchSettings GetSettings(Type fieldType) => new TypeSearchSettings(fieldType);

	public static void DrawButton(Rect pos, SerializedProperty property, GUIContent label, Type fieldType) {
		label.tooltip = (property.managedReferenceValue as IInspectorTooltip)?.Tooltip;
		//if(label.tooltip != null || true) {
		//	var infoRect = pos;
		//	infoRect.width = infoRect.height;
		//	infoRect.x -= 60;
		//	EditorGUI.LabelField(infoRect, new GUIContent("(?)", label.tooltip));
		//}

		if(GUI.Button(pos, label)) SearchProvider.Create(GetSettings(fieldType), SetValue).OpenWindow();
		void SetValue(object obj) {
			property.managedReferenceValue = Activator.CreateInstance((Type)obj);
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}
