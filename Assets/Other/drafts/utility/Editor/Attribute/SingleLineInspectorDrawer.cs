using UnityEngine;
using UnityEditor;
using System.Linq;
using Drafts;
using System.Collections.Generic;

namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(ISingleLineInspector), true)]
	public class SingleLineInspectorDrawer : PropertyDrawer {

		protected virtual List<string> names { get; } = new();
		protected virtual float RightPadding { get; }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(property.propertyType == SerializedPropertyType.ManagedReference
				&& property.managedReferenceValue is ISingleLineInspector obj) {
				position.width -= obj.RightPadding;

				if(obj.Configs != null) {
					DrawConfig(position, property, label, obj.Configs);
					return;
				}
			}

			var name = names.FirstOrDefault();

			if(name == null) property = property.GetChildren().FirstOrDefault();
			else property = property.FindPropertyRelative(name);

			EditorGUI.PropertyField(position, property, label);
		}

		void DrawConfig(Rect position, SerializedProperty property, GUIContent label, (string, string, float)[] configs) {
			EditorGUI.LabelField(position.NextX(EditorGUIUtility.labelWidth), label);
			var children = property.GetChildren();
			var totalWeigth = configs.Sum(c => c.Item3);
			var w = Mathf.Abs(position.width);

			for(int i = 0; i < configs.Length; i++) {
				var (name, alias, weight) = configs[i];
				var c = name == null ? children.ElementAt(i) : children.First(p => p.name == name);
				var l = alias == null ? GUIContent.none : new GUIContent(alias);
				var tw = w * weight / totalWeigth;
				var r = position.NextX(tw);
				EditorGUI.PropertyField(r, c, l);
			}
		}
	}

	[CustomPropertyDrawer(typeof(ISinglePropertyInspector), true)]
	public class SinglePropertyInspectorDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			string name = GetName(property);
			property = name == null
				? property.GetChildren().FirstOrDefault()
				: property.FindPropertyRelative(name);
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			string name = GetName(property);
			property = name == null
				? property.GetChildren().FirstOrDefault()
				: property.FindPropertyRelative(name);
			EditorGUI.PropertyField(position, property, label);
		}

		static string GetName(SerializedProperty property)
			=> property.propertyType == SerializedPropertyType.ManagedReference
			&& property.managedReferenceValue is ISinglePropertyInspector obj
			? obj.Name : null;
	}
}
