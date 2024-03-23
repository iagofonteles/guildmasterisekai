using Drafts.Editing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.Tilemap3D {

	public partial class Chunk {

		public void Clear() {
			tiles.Clear();
			ResizeMeshesList(new Tile[0]);
		}

		void QueueTiles(Vector3Int min, Vector3Int max) {
			for(int i = min.x - 1; i <= max.x + 1; i++)
				for(int j = min.y - 1; j <= max.y + 1; j++)
					for(int k = min.z - 1; k <= max.z + 1; k++) {
						var pos = new Vector3Int(i, j, k);
						var tile = this.GetTileRelative(pos, out var chunk) as ChunkTile;
						if(tile != null) manager.tilesToUpdate.Add(tile);
						if(chunk != null) manager.chunksToUpdate.Add(chunk as Chunk);
					}
		}

		public void Redraw() => ModifyMeshes(tiles.GroupBy(t => t.tile));

		void ModifyMeshes(IEnumerable<IGrouping<Tile, ChunkTile>> groups) {
			ResizeMeshesList(groups.Select(g => g.Key).ToArray());

			var index = 0;
			foreach(var group in groups) {
				var pairs = group.SelectMany(t => t.cornersRules.Select((c, i)
					=> (t.tile.Generator.GetMesh(i, c), Vector3.Scale(t.position, Manager.TileSize))));

				MeshUtil.CombineSubmeshes(meshes[index].mesh, pairs);
				meshes[index].materials = group.Key.Materials;
				//meshes[index].mesh.UploadMeshData(false);
				index++;
			}
			OnMeshModified?.Invoke();
		}

	}
}
