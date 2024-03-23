using Drafts.Editing;
using System;
using UnityEngine;
namespace Drafts.Tilemap3D {
	[Serializable]
	public class ChunkTile : ITileInstance, IComparable<Vector3Int> {
		public Tile tile;
		public Chunk chunk;
		public Vector3Int position;
		public Vector3 worldPosition;
		public int[] cornersRules = new int[0];
		public Material[] Materials => tile.Materials;

		public Tile Base => tile;
		public Chunk Chunk => chunk;
		public Vector3Int Position => position;
		public Vector3 WorldPosition => worldPosition;

		ITile ITileInstance.Base => tile;
		IChunk ITileInstance.Chunk => chunk;

		public ChunkTile(Chunk chunk, Vector3Int position) {
			this.chunk = chunk;
			this.position = position;
			worldPosition = chunk.WorldPosition + Vector3.Scale(position, chunk.Manager.TileSize);
		}

		public int CompareTo(Vector3Int other) => position.CompareTo(other);

		public void UpdateCorners() {
			if(!tile.Generator) return;
			var connections = tile.Generator.GetConnections(this);
			cornersRules = tile.Generator.GetCornersRule(connections);
			tile.Generator.Optimize(cornersRules);
		}

		void ITileInstance.OnPlaced(IChunk chunk, Vector3Int pos) {
			throw new NotImplementedException();
		}

		void ITileInstance.OnRemoved(IChunk chunk, Vector3Int pos) {
			throw new NotImplementedException();
		}
	}
}
