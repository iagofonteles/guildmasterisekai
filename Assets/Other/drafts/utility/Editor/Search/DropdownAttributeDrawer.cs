using Drafts;
using Drafts.Linq;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(DropdownAttribute), true)]
	public class DropdownAttributeDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var a = attribute as DropdownAttribute;
			EditorGUI.BeginProperty(position, label, property);
			Draw(position, property, label, a.Settings, a.Compare);
			EditorGUI.EndProperty();
		}

		public static void Draw(Rect position, SerializedProperty property, GUIContent label, ISearchSettings settings, Func<object, object, bool> compare) {
			var pos = position;
			var propW = pos.width - EditorGUIUtility.labelWidth - 35;

			EditorGUI.LabelField(pos.NextX(EditorGUIUtility.labelWidth), label);

			var itens = settings.GetItens().ToList();
			var options = itens.Select(settings.GetName).ToArray();

			var old = itens.IndexOf(property.GetValue());
			var current = EditorGUI.Popup(pos, null, old, options);

			if(current >= 0 && current != old)
				property.SetValue(itens[current]);
		}

	}
}
