//using UnityEditor;
//using Drafts;
//using System.Linq;
//using System.Collections.Generic;
//using Drafts.Patterns;
//using Drafts.Hidden;
//using UnityEngine;
//using System;
//using UnityEngine.UIElements;
//using DraftsEditor.UIElements;
//using UnityEditor.UIElements;

//namespace DraftsEditor {

//	public static class PrefabPoolEditor {

//		static Dictionary<string, Type> _dic;
//		static List<ClassTargetPair> _pool;

//		static Dictionary<string, Type> TypeDic => _dic ??= ReflectionUtil.FindDerivedTypes(typeof(IPreloaded)).ToDictionary(t => t.FullName, t => t);
//		static List<ClassTargetPair> Pool => _pool ??= DraftsSettings.Instance.Reflect<List<ClassTargetPair>>("pooled");

//		public static VisualElement GetVisualElement(SerializedObject serializedObject) {
//			var _pooled = serializedObject.FindProperty("pooled");
//			var container = new Header("Prefab Pool");
//			var prefabs = new VisualElement();

//			void Redraw() {
//				serializedObject.Update();
//				prefabs.Clear();

//				for(int i = 0; i < _pooled.arraySize; i++) {
//					var _type = _pooled.GetArrayElementAtIndex(i).FindPropertyRelative("type");
//					var _target = _pooled.GetArrayElementAtIndex(i).FindPropertyRelative("target");

//					void Validate(object value) {
//						_target.objectReferenceValue = (UnityEngine.Object)value;
//						serializedObject.ApplyModifiedProperties();
//						Redraw();
//					}

//					var obj = new ObjectField(_type.stringValue);
//					obj.objectType = typeof(GameObject);
//					obj.allowSceneObjects = false;
//					obj.value = (_target.objectReferenceValue as Component)?.gameObject;
//					obj.OnChanged(Validate);
//					prefabs.Add(obj);

//					obj.Q<Image>().parent.Click(ev => SearchProvider.Create(new ComponentSearchSettings(
//							TypeDic[_type.stringValue], DraftsSettings.Folder), Validate).OpenWindow());
//				}
//			}

//			Redraw();
//			container.AddButton("Refresh", () => { RefreshS(); Redraw(); });
//#if !DRAFTS_USE_ADDRESSABLES
//			container.AddButton("Find in Project", () => { FindInProject(); Redraw(); });
//#endif
//			container.Add(prefabs);
//			return container;
//		}

//		// check if all and only one prefab is assigned for each IPrefabPool
//		static void RefreshS() {
//			var allTypes = ReflectionUtil.FindDerivedTypes(typeof(IPreloaded));
//			Pool.RemoveAll(p => !allTypes.Any(t => t.FullName == p.type));
//			allTypes.Where(t => !Pool.Any(p => p.type == t.FullName));
//			foreach(var t in allTypes) Pool.Add(new ClassTargetPair { type = t.FullName });
//		}

//		static void FindInProject() {
//#if !DRAFTS_USE_ADDRESSABLES
//			EditorUtility.DisplayProgressBar("Searching", null, 1);
//			var allInProject = EditorUtil.FindPrefabsWith(typeof(IPreloaded), null, DraftsSettings.Folder);
//			foreach(var c in allInProject) {
//				var p = Pool.FirstOrDefault(p => p.type == c.GetType().FullName);
//				if(p != null) p.target = c;
//			}
//			EditorUtility.ClearProgressBar();
//			EditorUtility.SetDirty(DraftsSettings.Instance);
//#endif
//		}

//	}
//}
