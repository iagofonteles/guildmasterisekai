using UnityEngine;
using UnityEditor;

namespace DraftsEditor.Editing {

	public class Line {
		public float thickness = 1;
		public bool dotted;
		public float size;
		public Vector3 direction;
		public Color color = Color.white;
		public Vector3 Position;

		public void Draw() {
			Handles.color = color;
			if(dotted) Handles.DrawDottedLine(Position + direction * size / 2, Position - direction * size / 2, thickness);
			else Handles.DrawLine(Position + direction * size / 2, Position - direction * size / 2, thickness);
		}
	}
}
