using Drafts.Editing;
using UnityEngine;
namespace DraftsEditor.Editing {
	public class Manipulator : IManipulator {
		public IGizmo Gizmo { get; } = new CubeLine();
		public ITile Tile { get; set; }
		public IChunkManager ChunkManager { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 FreePosition { get; set; }
	}
}
