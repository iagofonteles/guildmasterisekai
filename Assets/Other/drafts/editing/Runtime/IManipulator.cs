using UnityEngine;
namespace Drafts.Editing {
	public interface IManipulator {
		IGizmo Gizmo { get; }
		ITile Tile { get; set; }
		IChunkManager ChunkManager { get; set; }
		Vector3 Position { get; set; }
		Vector3 FreePosition { get; set; }
		void SetTile(ITile tile) => Tile = tile;
	}
}