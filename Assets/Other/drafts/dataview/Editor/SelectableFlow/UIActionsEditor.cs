using UnityEditor;
using Drafts.UI.Flow;
using UnityEditor.UI;
using System.Linq;
namespace DraftsEditor {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIActions), true)]
	public class UIActionsEditor : SelectableEditor {

		SerializedProperty actions;
		SerializedProperty preset;
		SerializedProperty register;
		SerializedProperty targetGraphic;
		SerializedProperty interactable;
		SerializedProperty navigation;
		SerializedProperty onInteractableChanged;

		protected override void OnEnable() {
			base.OnEnable();
			preset = serializedObject.FindProperty("preset");
			register = serializedObject.FindProperty("registerGlobally");
			actions = serializedObject.FindProperty("actions");
			targetGraphic = serializedObject.FindProperty("m_TargetGraphic");
			interactable = serializedObject.FindProperty("m_Interactable");
			navigation = serializedObject.FindProperty("m_Navigation");
			onInteractableChanged = serializedObject.FindProperty("OnInteractableChanged");
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.PropertyField(register);
			EditorGUILayout.PropertyField(preset);
			serializedObject.ApplyModifiedProperties();
			if(preset.objectReferenceValue) {
				EditorGUILayout.PropertyField(interactable);
				EditorGUILayout.PropertyField(navigation);
			} else {
				EditorGUILayout.PropertyField(targetGraphic);
				base.OnInspectorGUI();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}
			if(targets.Length > 1) {
				EditorGUILayout.HelpBox("Cannot edit multiple objects.", MessageType.Info);
				return;
			}

			var tgt = serializedObject.targetObject as UIActions;
			var options = UIAction.ActionMap.actions.Select(a => a.name)
				.Where(s => !tgt.Actions.Any(a => a.Control == s)).ToArray();
			var choose = EditorGUILayout.Popup("Add Control", -1, options);
			if(choose >= 0) {
				actions.arraySize++;
				var a = actions.GetArrayElementAtIndex(actions.arraySize - 1);
				a.FindPropertyRelative("control").stringValue = options[choose];
				a.isExpanded = true;
			}
			EditorGUILayout.PropertyField(actions);
			EditorGUILayout.PropertyField(onInteractableChanged);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
