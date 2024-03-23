using UnityEditor;
using UnityEngine;
using Drafts;

namespace DraftsEditor {
    [CustomPropertyDrawer(typeof(Drafts.Patterns.Conditional<,>), true)]
    public class ConditionalValueDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var active = property.FindPropertyRelative("active");
            var value = property.FindPropertyRelative("value");
            var rect = new ControlRect(position);

            if (active.boolValue) {
                EditorGUI.PropertyField(rect.MoveX(rect.width/2), active, label);;
				rect.MoveX(8);
				EditorGUI.PropertyField(rect.NextLine(), value, GUIContent.none, true);
            }
            else EditorGUI.PropertyField(position, active, label);
        }
    }

}
