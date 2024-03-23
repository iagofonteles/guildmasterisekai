using System.Collections.Generic;
using UnityEngine;

namespace Drafts.Editing {
	public interface IChunkManager : IEnumerable<IChunk> {
		Vector3Int ChunkLength { get; }
		Vector3 TileSize => Vector3.one;
		IChunk GetChunk(Vector3Int chunkKey);
		IChunk CreateChunk(Vector3Int chunkKey);
		void Update();
	}

	public static class ExtensionsIChunkManager {

		public static Vector3Int GetChunkKey(this IChunkManager m, Vector3 worldPosition, out Vector3Int localPosition) {
			var chunkSize = Vector3.Scale(m.ChunkLength, m.TileSize);
			var chunkKey = new Vector3Int(
				Mathf.FloorToInt(worldPosition.x / chunkSize.x),
				Mathf.FloorToInt(worldPosition.y / chunkSize.y),
				Mathf.FloorToInt(worldPosition.z / chunkSize.z)
			);
			localPosition = new Vector3Int(
				Mathf.FloorToInt(worldPosition.x / m.TileSize.x - chunkKey.x * m.ChunkLength.x),
				Mathf.FloorToInt(worldPosition.y / m.TileSize.y - chunkKey.y * m.ChunkLength.y),
				Mathf.FloorToInt(worldPosition.z / m.TileSize.z - chunkKey.z * m.ChunkLength.z)
			);
			return chunkKey;
		}

		public static Vector3Int GetChunkKey(this IChunkManager m, Vector3Int gridPosition, out Vector3Int localPosition) {
			Vector3Int chunkKey = new Vector3Int(
				Mathf.FloorToInt((float)gridPosition.x / m.ChunkLength.x),
				Mathf.FloorToInt((float)gridPosition.y / m.ChunkLength.y),
				Mathf.FloorToInt((float)gridPosition.z / m.ChunkLength.z)
			);
			localPosition = new Vector3Int(
				gridPosition.x - chunkKey.x * m.ChunkLength.x,
				gridPosition.y - chunkKey.y * m.ChunkLength.y,
				gridPosition.z - chunkKey.z * m.ChunkLength.z
			);
			return chunkKey;
		}

		public static Vector3Int GetGridPosition(this IChunkManager m, Vector3 worldPos) {
			var cKey = m.GetChunkKey(worldPos, out var local);
			var pos = Vector3Int.Scale(cKey, m.ChunkLength);
			return pos + local;
		}

		public static IChunk GetChunk(this IChunkManager m, Vector3 worldPosition)
			=> m.GetChunk(GetChunkKey(m, worldPosition, out _));

		public static ITileInstance GetTile(this IChunkManager m, Vector3 worldPosition, out IChunk chunk) {
			chunk = m.GetChunk(GetChunkKey(m, worldPosition, out var pos));
			return chunk?.GetTile(pos);
		}

		public static ITileInstance GetTile(this IChunkManager m, Vector3Int gridPosition, out IChunk chunk) {
			chunk = m.GetChunk(GetChunkKey(m, gridPosition, out var localPos));
			return chunk?.GetTile(localPos);
		}

		public static void SetTile(this IChunkManager m, Vector3 worldPosition, ITile tile)
			=> m.CreateChunk(GetChunkKey(m, worldPosition, out var pos)).SetTile(pos, tile);

		public static Vector3 SnapToGridCenter(this IChunkManager m, Vector3 worldPosition) => new Vector3(
			Mathf.Round(worldPosition.x / m.TileSize.x) * m.TileSize.x,
			Mathf.Round(worldPosition.y / m.TileSize.y) * m.TileSize.y,
			Mathf.Round(worldPosition.z / m.TileSize.z) * m.TileSize.z
		);

		public static Vector3 SnapToGrid(this IChunkManager m, Vector3 worldPosition) => new Vector3(
			Mathf.Floor(worldPosition.x / m.TileSize.x) * m.TileSize.x + m.TileSize.x / 2,
			Mathf.Floor(worldPosition.y / m.TileSize.y) * m.TileSize.y + m.TileSize.y / 2,
			Mathf.Floor(worldPosition.z / m.TileSize.z) * m.TileSize.z + m.TileSize.z / 2
		);

	}
}
