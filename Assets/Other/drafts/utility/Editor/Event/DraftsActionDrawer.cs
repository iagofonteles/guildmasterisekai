using Drafts;
using Drafts.EventExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DraftsEditor {
	[CustomPropertyDrawer(typeof(DraftsAction), true)]
	public class DraftsActionDrawer : PropertyDrawer {
		static BindingFlags filter = BindingFlags.DeclaredOnly | BindingFlags.Static
			| BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var obj = property.FindPropertyRelative("obj");
			var args = property.FindPropertyRelative("args");
			return base.GetPropertyHeight(property, label) * (1 + args.arraySize
				+ (obj.objectReferenceValue ? 1 : 0));
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			var signature = property.FindPropertyRelative("signature");
			var obj = property.FindPropertyRelative("obj");
			var method = property.FindPropertyRelative("method");
			var inline = property.FindPropertyRelative("inline");
			var args = property.FindPropertyRelative("args");

			var type = obj.objectReferenceValue ? obj.objectReferenceValue.GetType() : null;
			var search = new MethodSearchSettings(type, filter, signature.stringValue, inline.boolValue);
			var methodInfo = search.GetMethod(method.stringValue);

			position.height /= 1 + args.arraySize + (obj.objectReferenceValue ? 1 : 0);

			DrawHeader(position, label, obj, inline, ResetArgs);
			if(!obj.objectReferenceValue) return;
			position.y += position.height;

			DrawMethodBtn(position, method, search, SelectMethod);
			if(methodInfo == null) return;
			position.y += position.height;

			DrawArgs(position, args, methodInfo.GetParameters(), method);

			EditorGUI.EndProperty();

			void SelectMethod(MethodInfo m) {
				method.SetValue(search.GetName(m));
				methodInfo = m;
				ResetArgs();
			}

			void ResetArgs() {
				if(inline.boolValue && !methodInfo.ValidParameters()) {
					method.stringValue = "";
					args.arraySize = 0;
					methodInfo = null;
				}
				if(!inline.boolValue) args.arraySize = 0;
				else DraftsArgDrawer.Reset(args, methodInfo?.GetParameters());
			}
		}

		void DrawHeader(Rect position, GUIContent label, SerializedProperty obj, SerializedProperty inline, Action resetArgs) {
			var iw = 48;
			var tw = 16;
			var lw = (position.width - iw - tw) / 3;
			var ow = (position.width - iw - tw) * 2 / 3;

			position.width = lw;
			EditorGUI.LabelField(position, label);

			position.x += position.width;
			position.width = ow;
			var gcBtnPos = position;
			gcBtnPos.width = gcBtnPos.height;
			gcBtnPos.x -= gcBtnPos.width;

			DrawGetComponentButton(gcBtnPos, obj);
			EditorGUI.PropertyField(position, obj, GUIContent.none);

			if(!obj.objectReferenceValue) return;

			position.x += position.width;
			position.width = iw;
			EditorGUI.LabelField(position, "  inline");

			position.x += position.width;
			position.width = tw;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, inline, GUIContent.none);
			if(EditorGUI.EndChangeCheck()) resetArgs();
		}

		void DrawGetComponentButton(Rect position, SerializedProperty obj) {
			if(obj.objectReferenceValue is GameObject go)
				if(GUI.Button(position, "+"))
					new ComponentSearchSettings(go)
						.Search<Component>(obj.SetValue);
			if(obj.objectReferenceValue is Component c)
				if(GUI.Button(position, "-"))
					obj.SetValue(c.gameObject);
		}

		private void DrawMethodBtn(Rect position, SerializedProperty method, MethodSearchSettings search, Action<MethodInfo> onSelect) {
			EditorGUI.indentLevel++;
			var btnw = position.width - EditorGUIUtility.labelWidth;

			position.width = EditorGUIUtility.labelWidth;
			EditorGUI.LabelField(position, "Method");

			position.x += position.width;
			position.width = btnw;
			var plusBtnPos = position;
			plusBtnPos.width = plusBtnPos.height + 18;
			plusBtnPos.x -= plusBtnPos.width;
			filter = (BindingFlags)EditorGUI.EnumFlagsField(plusBtnPos, GUIContent.none, filter);

			if(GUI.Button(position, method.stringValue)) search.Search(onSelect);
			EditorGUI.indentLevel--;
		}

		void DrawArgs(Rect position, SerializedProperty args, ParameterInfo[] param, SerializedProperty method) {
			EditorGUI.indentLevel++;
			for(int i = 0; i < args.arraySize; i++) {
				var a = args.GetArrayElementAtIndex(i);
				var t = param[i];
				DraftsArgDrawer.Draw(position, a, new GUIContent(t.Name), t.ParameterType);
				position.y += position.height;
			}
			EditorGUI.indentLevel--;
		}

		class ComponentSearchSettings : ISearchSettings<Component> {
			GameObject go;
			public ComponentSearchSettings(GameObject go) => this.go = go;
			public string Title => "Components in " + go.name;
			public IEnumerable<Component> GetItens() => go.GetComponents<Component>();
			public string GetName(Component obj) => obj.GetType().Name;
		}

		class MethodSearchSettings : ISearchSettings<MethodInfo> {
			public string Title { get; }
			MethodInfo[] itens { get; }

			public MethodSearchSettings(Type type, BindingFlags flags, string signature, bool inline) {
				if(type == null) {
					itens = new MethodInfo[0];
					return;
				}
				var methods = type.GetMethods(flags).Where(ValidMethod).Where(signature.FlexibleSignature);
				if(inline) methods = methods.Where(DraftsActionExtras.ValidParameters);
				itens = methods.ToArray();
				Title = type.Name;
			}

			IEnumerable<MethodInfo> ISearchSettings<MethodInfo>.GetItens() => itens;
			public string GetName(MethodInfo m) => m?.GetNameWithParams();
			public MethodInfo GetMethod(string nameArgs) => itens.FirstOrDefault(nameArgs.MatchMethod);

			bool ValidMethod(MethodInfo m) => !m.IsConstructor;
		}
	}
}
