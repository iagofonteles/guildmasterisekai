using Drafts.Editing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Drafts.Tilemap3D {

	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/Tilemap")]
	public class Tilemap : ScriptableObject, IChunkManager, IEnumerable<Chunk>, IEnumerable<IChunk> {
		[SerializeField] Vector3Int chunkLength = new(10, 10, 10);
		[SerializeField] Vector3 tileSize = new(1, 1, 1);
		[SerializeField] List<Chunk> chunks;

		public Vector3Int ChunkLength => chunkLength;
		public Vector3 TileSize => tileSize;
		internal HashSet<ChunkTile> tilesToUpdate = new();
		internal HashSet<Chunk> chunksToUpdate = new();

		public IChunk GetChunk(Vector3Int key) => chunks.BinaryFindOrDefault(key);
		public IChunk CreateChunk(Vector3Int key) {
			var index = chunks.BinarySearch(key);
			if(index < 0) {
				index = ~index;
				var chunk = Chunk.Create(this, key);
				chunks.Insert(index, chunk);
				OnChunkAdded?.Invoke(chunk);
			}
			return chunks[index];
		}

		public event Action<Chunk> OnChunkAdded;
		public event Action<Chunk> OnChunkRemoved;

		public void Update() {
			foreach(var tile in tilesToUpdate) tile.UpdateCorners();
			foreach(var chunk in chunksToUpdate) chunk.Redraw();
			tilesToUpdate.Clear();
			chunksToUpdate.Clear();
		}

		public void Recalculate() {
			foreach(var chunk in chunks)
				foreach(var tile in chunk) tile.UpdateCorners();
		}

		public void Redraw() {
			foreach(var chunk in chunks)
				chunk.Redraw();
		}

		void Reset() => Clear();
		public void Clear() {
			foreach(var chunk in chunks) {
				chunk.Clear();
#if UNITY_EDITOR
				UnityEditor.AssetDatabase.RemoveObjectFromAsset(chunk);
				DraftsUtil.DestroyHybrid(chunk);
#endif
				OnChunkRemoved?.Invoke(chunk);
			}
			chunks.Clear();
#if UNITY_EDITOR
			var all = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(this));
			foreach(var item in all) if(item != this) {
					UnityEditor.AssetDatabase.RemoveObjectFromAsset(item);
					DraftsUtil.DestroyHybrid(item);
				}
			UnityEditor.AssetDatabase.SaveAssets();
#endif
		}

		public IEnumerator<Chunk> GetEnumerator() => chunks.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => chunks.GetEnumerator();
		IEnumerator<IChunk> IEnumerable<IChunk>.GetEnumerator() => chunks.GetEnumerator();
	}
}
