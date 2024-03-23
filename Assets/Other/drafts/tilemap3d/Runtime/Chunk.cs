using Drafts.Editing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drafts.Tilemap3D {

	[Serializable]
	public class ChunkMesh {
		public Mesh mesh;
		public Material[] materials = new Material[0];
	}

	public partial class Chunk : ScriptableObject, IChunk, IComparable<Vector3Int>, IEnumerable<ChunkTile>, IEnumerable<ITileInstance> {
		[SerializeField] Tilemap manager;
		[SerializeField] Vector3Int key;
		[SerializeField] Vector3 worldPosition;
		[SerializeField] List<ChunkTile> tiles = new();
		[SerializeField] List<ChunkMesh> meshes = new();

		public IChunkManager Manager => manager;
		public Vector3Int Key => key;
		public Vector3 WorldPosition => worldPosition;
		public List<ChunkMesh> Meshes => meshes;

		public event Action<Vector3Int, ChunkTile> OnTileChanged;
		public event Action OnMeshAdded;
		public event Action OnMeshRemoved;
		public event Action OnMeshModified;

		internal static Chunk Create(Tilemap manager, Vector3Int key) {
			var chunk = CreateInstance<Chunk>();
			chunk.name = key.ToString();
			chunk.manager = manager;
			chunk.key = key;
			chunk.worldPosition = Vector3.Scale(key * manager.ChunkLength, manager.TileSize);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.AddObjectToAsset(chunk, manager);
#endif
			return chunk;
		}

		public ITileInstance GetTile(Vector3Int localPosition) => tiles.BinaryFindOrDefault(localPosition);

		public void SetTile(Vector3Int pos, ITile tile) {
			if(!this.InBounds(pos)) Debug.LogError(pos, this);
			var index = tiles.BinarySearch(pos);

			if(tile == null) {
				if(index >= 0) {
					tiles.RemoveAt(index);
					OnTileChanged?.Invoke(pos, null);
					QueueTiles(pos, pos);
				}
				return;
			}

			if(tile is not Tile) throw new Exception("tile is not Tile3D.");

			if(index < 0) {
				index = ~index;
				tiles.Insert(index, new ChunkTile(this, pos));
			}

			tiles[index].tile = tile as Tile;
			OnTileChanged?.Invoke(pos, tiles[index]);
			QueueTiles(pos, pos);
		}

		void ResizeMeshesList(Tile[] tiles) {
			while(meshes.Count > tiles.Length) {
#if UNITY_EDITOR
				UnityEditor.AssetDatabase.RemoveObjectFromAsset(meshes[meshes.Count - 1].mesh);
				DraftsUtil.DestroyHybrid(meshes[meshes.Count - 1].mesh);
#endif
				meshes.RemoveAt(meshes.Count - 1);
				OnMeshRemoved?.Invoke();
			}
			while(meshes.Count < tiles.Length) {
				var mesh = new Mesh() {
					name = $"{key} {meshes.Count}",
					indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
				};
#if UNITY_EDITOR
				UnityEditor.AssetDatabase.AddObjectToAsset(mesh, this);
#endif
				meshes.Add(new ChunkMesh {
					mesh = mesh,
					materials = tiles[meshes.Count].Materials
				});
				OnMeshAdded?.Invoke();
			}
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.SaveAssets();
#endif
		}

		public int CompareTo(Vector3Int other) => Key.CompareTo(other);
		public IEnumerator<ChunkTile> GetEnumerator() => tiles.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => tiles.GetEnumerator();
		IEnumerator<ITileInstance> IEnumerable<ITileInstance>.GetEnumerator() => tiles.GetEnumerator();
	}
}
