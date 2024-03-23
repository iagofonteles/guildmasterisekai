using UnityEditor;
using UnityEngine;
using Drafts;
using UnityEditorInternal;
using System;

namespace DraftsEditor {

#if DRAFTS_MINIMIZE_UNITY_EVENTS
    [CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEventBase), true)]
#endif
	//[CustomPropertyDrawer(typeof(MinimizeEventAttribute))]
	//public class DefaultMinimizeEventAttributeDrawer : UnityEventDrawer {
	//	bool init;

	//	void Init(SerializedProperty property) {
	//		if(init) return;
	//		property.isExpanded = MinimizeEventAttribute.GetFoldoutFor(property);
	//		init = true;
	//	}

	//	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
	//		return property.isExpanded ? base.GetPropertyHeight(property, label) : 20;
	//	}

	//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
	//		Init(property);
	//		if(property.isExpanded) base.OnGUI(position, property, label);
	//		else if(GUI.Button(position, "+ " + label.text)) property.isExpanded = true;
	//	}
	//}

	//[CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEventBase), true)]
	//[CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEvent))]
	//[CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEvent<>))]
	//[CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEvent<,>))]
	//[CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEvent<,,>))]
	//[CustomPropertyDrawer(typeof(UnityEngine.Events.UnityEvent<,,,>))]
	[CustomPropertyDrawer(typeof(MinimizeEventAttribute))]
	public class MinimizeEventAttributeDrawer : UnityEventDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			Debug.Log("ping");
			return property.isExpanded ? base.GetPropertyHeight(property, label) : 20;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Debug.Log("aaw");
			foldout = HasEvents(property);
			property.isExpanded = foldout.Value;
			if(foldout.Value) base.OnGUI(position, property, label);
			else if(GUI.Button(position, "+ " + label.text)) foldout = true;
		}

		bool? foldout;
		static bool HasEvents(SerializedProperty p) => p.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize > 0;
	}
}
