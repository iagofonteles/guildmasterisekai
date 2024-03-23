using UnityEngine;
using UnityEditor;
using Drafts.Editing;

namespace DraftsEditor.Editing {

	public class CubeLine : IGizmo {
		public float thickness = 1;
		public Vector3 Size { get; set; } = Vector3.one;
		public Color Color { get; set; } = Color.white;
		public Vector3 Position { get; set; }

		public void Draw() {
			Handles.color = Color;
			var s = Size / 2;
			var a = Position + new Vector3(-s.x, -s.y, -s.z);
			var b = Position + new Vector3(s.x, -s.y, -s.z);
			var c = Position + new Vector3(-s.x, s.y, -s.z);
			var d = Position + new Vector3(s.x, s.y, -s.z);

			var e = Position + new Vector3(-s.x, -s.y, s.z);
			var f = Position + new Vector3(s.x, -s.y, s.z);
			var g = Position + new Vector3(-s.x, s.y, s.z);
			var h = Position + new Vector3(s.x, s.y, s.z);

			Handles.DrawLine(a, b, thickness);
			Handles.DrawLine(c, d, thickness);
			Handles.DrawLine(a, c, thickness);
			Handles.DrawLine(b, d, thickness);

			Handles.DrawLine(e, f, thickness);
			Handles.DrawLine(g, h, thickness);
			Handles.DrawLine(e, g, thickness);
			Handles.DrawLine(f, h, thickness);

			Handles.DrawLine(a, e, thickness);
			Handles.DrawLine(b, f, thickness);
			Handles.DrawLine(c, g, thickness);
			Handles.DrawLine(d, h, thickness);
		}
	}
}
