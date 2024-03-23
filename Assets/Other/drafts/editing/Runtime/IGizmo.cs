using UnityEngine;

namespace Drafts.Editing {
	public interface IGizmo {
		Vector3 Position { get; set; }
		Vector3 Size { get; set; }
		Color Color { get; set; }
		void Draw() { }
	}
}