using UnityEditor;
using UnityEngine.UI;
namespace DraftsEditor {

	[CustomEditor(typeof(SelectablePreset), true)]
	public class SelectablePresetEditor : Editor {
		SerializedProperty transition;
		SerializedProperty colors;
		SerializedProperty sprites;
		SerializedProperty triggers;

		void OnEnable() {
			transition = serializedObject.FindProperty("transition");
			colors = serializedObject.FindProperty("colors");
			sprites = serializedObject.FindProperty("sprites");
			triggers = serializedObject.FindProperty("triggers");
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.PropertyField(transition, true);
			var value = (Selectable.Transition)transition.enumValueIndex;
			switch(value) {
				case Selectable.Transition.ColorTint: EditorGUILayout.PropertyField(colors, true); break;
				case Selectable.Transition.SpriteSwap: EditorGUILayout.PropertyField(sprites, true); break;
				case Selectable.Transition.Animation: EditorGUILayout.PropertyField(triggers, true); break;
				default: break;
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
