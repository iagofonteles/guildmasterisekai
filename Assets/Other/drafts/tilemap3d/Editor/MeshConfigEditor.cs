using UnityEngine;
using UnityEditor;
using Drafts;
using Drafts.Tilemap3D.Generators.Corners;

[CustomPropertyDrawer(typeof(MeshConfig))]
public class MeshConfigEditor : PropertyDrawer {

	float spacing = 4;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		var subMeshes = property.FindPropertyRelative("subMeshes");
		var h = base.GetPropertyHeight(property, label);
		return property.isExpanded ? h * (1 + subMeshes.arraySize) + spacing : h;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		var title = label.text.Substring("Element".Length);
		var mesh = property.FindPropertyRelative("mesh");
		var subMeshes = property.FindPropertyRelative("subMeshes");

		var sSize = mesh.objectReferenceValue ? ((Mesh)mesh.objectReferenceValue).subMeshCount : 0;
		if(subMeshes.arraySize != sSize) subMeshes.arraySize = sSize;

		var r = new ControlRect(position);
		if(property.isExpanded) r.current.height = (r.current.height - spacing) / (1 + subMeshes.arraySize);

		// title / mesh / swap / rule
		property.isExpanded = EditorGUI.Foldout(r.MoveX(20), property.isExpanded, title, true);
		EditorGUI.ObjectField(r.NextLine(), mesh, GUIContent.none);

		if(property.isExpanded) {
			// sub meshes
			r.current.y += spacing / 2;
			r.Indent(30);
			for(int i = 0; i < subMeshes.arraySize; i++) {
				var p = subMeshes.GetArrayElementAtIndex(i);
				EditorGUI.PropertyField(r.NextLine(), p, new GUIContent(i.ToString()), false);
			}
			r.Indent(-30);
			//EditorGUI.LabelField(r.NextLine(), "", GUI.skin.horizontalSlider);
		}

		EditorGUI.EndProperty();
	}
}
