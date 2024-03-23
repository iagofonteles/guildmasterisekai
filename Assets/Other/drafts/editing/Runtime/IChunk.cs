using System.Collections.Generic;
using UnityEngine;

namespace Drafts.Editing {
	public interface IChunk : IEnumerable<ITileInstance> {
		IChunkManager Manager { get; }
		Vector3Int Key { get; }
		ITileInstance GetTile(Vector3Int pos);
		void SetTile(Vector3Int pos, ITile tile);
	}

	public static class ExtensionsIChunk {
		public static bool InBounds(this IChunk c, Vector3Int localPosition)
			=> localPosition.x >= 0 && localPosition.x < c.Manager.ChunkLength.x
			&& localPosition.y >= 0 && localPosition.y < c.Manager.ChunkLength.y
			&& localPosition.z >= 0 && localPosition.z < c.Manager.ChunkLength.z;

		/// <summary>Can return tiles outside this chunk, using relative position</summary>
		public static ITileInstance GetTileRelative(this IChunk c, Vector3Int localPosition, out IChunk chunk)
			=> c.InBounds(localPosition) ? (chunk = c).GetTile(localPosition)
			: c.Manager.GetTile(Vector3Int.Scale(c.Key, c.Manager.ChunkLength) + localPosition, out chunk);
	}
}
