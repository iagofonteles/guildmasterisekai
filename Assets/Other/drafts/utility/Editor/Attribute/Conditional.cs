//using UnityEditor;
//using UnityEngine;
//using Drafts;
//using UnityEngine.UIElements;
//using UnityEditor.UIElements;
//using System;

//namespace DraftsEditor {

//	[CustomPropertyDrawer(typeof(ConditionalAttribute))]
//	public class ConditionalAttributeDrawer : PropertyDrawer {

//		bool GetValue(SerializedProperty property, ConditionalAttribute cond) {
//			var p = property.SiblingProperty(cond.field);

//			if(p == null) {
//				Debug.LogError($"field {cond.field} not found", property.serializedObject.targetObject);
//				return true;
//			}

//			return p.propertyType switch {
//				SerializedPropertyType.Boolean => cond.Validate(p.boolValue),
//				SerializedPropertyType.Enum => cond.Validate(p.enumValueIndex),
//				SerializedPropertyType.Integer => cond.Validate(p.intValue),
//				SerializedPropertyType.Float => cond.Validate(p.floatValue),
//				SerializedPropertyType.String => cond.Validate(p.stringValue),
//				SerializedPropertyType.ObjectReference => cond.Validate(p.objectReferenceValue),
//				_ => throw new Exception("Invalid Type:" + p.propertyType)
//			};
//		}

//		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//			=> GetValue(property, attribute as ConditionalAttribute) ? EditorGUI.GetPropertyHeight(property) : 0;

//		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//			try {
//				if(GetValue(property, attribute as ConditionalAttribute)) EditorGUI.PropertyField(position, property, label, true);
//			} catch(Exception e) {
//				Debug.LogWarning("error on ConditionalAttribute drawer: " + e.Message);
//			}
//		}

//		public override VisualElement CreatePropertyGUI(SerializedProperty property)
//			=> GetValue(property, attribute as ConditionalAttribute) ? new PropertyField(property) : null;
//	}
//}

