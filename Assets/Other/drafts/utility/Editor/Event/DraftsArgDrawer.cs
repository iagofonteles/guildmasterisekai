using Drafts;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(DraftsArg), true)]
	public class DraftsArgDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var type = property.FindPropertyRelative("type");
			var t = Type.GetType(type.stringValue);
			if(t == null) throw new Exception("Cant find type " + type.stringValue);
			Draw(position, property, label, t);
		}

		public static void Draw(Rect position, SerializedProperty property, GUIContent label, Type t) {
			EditorGUI.BeginProperty(position, label, property);
			var type = property.FindPropertyRelative("type");

			if(typeof(UnityEngine.Object).IsAssignableFrom(t)) {
				var prop = property.FindPropertyRelative("objValue");
				EditorGUI.ObjectField(position, prop, t, label);
			} else {
				var prop = property.FindPropertyRelative("value");
				DrawStringAsType(position, prop, label, t.Name);
			}
			EditorGUI.EndProperty();
		}

		static void DrawStringAsType(Rect position, SerializedProperty prop, GUIContent label, string type) {
			void Draw<T>(Func<Rect, GUIContent, T, T> gui, Func<string, T> parse)
				=> prop.stringValue = gui(position, label, parse(prop.stringValue))?.ToString();

			switch(type) {
				case "String": EditorGUI.PropertyField(position, prop, label); break;
				case "Int32": Draw(EditorGUI.IntField, int.Parse); break;
				case "Single": Draw(EditorGUI.FloatField, float.Parse); break;
				case "Boolean": Draw(EditorGUI.Toggle, bool.Parse); break;
				default: throw new Exception("type not supported: " + type);
			}
		}

		static void Reset(SerializedProperty arg, Type type) {
			arg.FindPropertyRelative("type").stringValue = type.Name;
			arg.FindPropertyRelative("objValue").objectReferenceValue = null;
			arg.FindPropertyRelative("value").stringValue =
				(type.IsValueType ? Activator.CreateInstance(type) : null)?.ToString();
		}

		public static void Reset(SerializedProperty args, ParameterInfo[] infos) {
			if(infos == null) { args.arraySize = 0; return; }
			args.arraySize = infos.Length;
			for(int i = 0; i < args.arraySize; i++)
				Reset(args.GetArrayElementAtIndex(i), infos[i].ParameterType);
			args.serializedObject.ApplyModifiedProperties();
		}

	}
}
