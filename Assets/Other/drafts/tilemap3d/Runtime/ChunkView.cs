using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drafts.Tilemap3D {

	[Serializable]
	class ChunkTilePrefab : IComparable<Vector3Int>, IComparable<ChunkTilePrefab> {
		public ChunkTile tile;
		public GameObject go;
		public int CompareTo(Vector3Int other) => tile.position.CompareTo(other);
		public int CompareTo(ChunkTilePrefab other) => tile.position.CompareTo(other.tile.position);
	}

	public class ChunkView : MonoBehaviour, IComparable<ChunkView>, IComparable<Chunk> {
		[SerializeField] Chunk chunk;
		[SerializeField] List<GameObject> meshes = new();
		[SerializeField] List<ChunkTilePrefab> prefabInstance = new();

		public Chunk Chunk {
			get => chunk;
			set {
				if(chunk == value) return;
				Unsubscribe();
				chunk = value;
				Subscribe();
				Refresh();
			}
		}

		public void Subscribe() {
			if(chunk == null) return;
			chunk.OnMeshAdded -= Refresh;
			chunk.OnMeshAdded += Refresh;
			chunk.OnMeshRemoved -= Refresh;
			chunk.OnMeshRemoved += Refresh;
			chunk.OnMeshModified -= UpdateColliders;
			chunk.OnMeshModified += UpdateColliders;
			chunk.OnTileChanged -= UpdatePrefabs;
			chunk.OnTileChanged += UpdatePrefabs;
		}

		public void Unsubscribe() {
			if(chunk == null) return;
			chunk.OnMeshAdded -= Refresh;
			chunk.OnMeshRemoved -= Refresh;
			chunk.OnMeshModified -= UpdateColliders;
			chunk.OnTileChanged -= UpdatePrefabs;
		}

		public void Refresh() {
			for(int i = 0; i < chunk.Meshes.Count; i++) {
				if(i >= meshes.Count) {
					var go = new GameObject("mesh " + i);
					go.AddComponent<MeshCollider>();
					go.AddComponent<MeshFilter>();
					go.AddComponent<MeshRenderer>();
					go.transform.parent = transform;
					go.transform.localPosition = Vector3.zero;
					meshes.Add(go);
				}
				meshes[i].GetComponent<MeshFilter>().sharedMesh = chunk.Meshes[i].mesh;
				meshes[i].GetComponent<MeshCollider>().sharedMesh = chunk.Meshes[i].mesh;
				meshes[i].GetComponent<MeshRenderer>().materials = chunk.Meshes[i].materials;
			}
			while(meshes.Count > chunk.Meshes.Count) {
				DraftsUtil.DestroyHybrid(meshes[meshes.Count - 1]);
				meshes.RemoveAt(meshes.Count - 1);
			}
		}

		public void UpdateColliders() {
			for(int i = 0; i < meshes.Count; i++) {
				var mesh = chunk.Meshes[i].mesh;
				meshes[i].GetComponent<MeshCollider>().sharedMesh
					= mesh.vertexCount > 0 ? mesh : null;
			}
		}

		private void UpdatePrefabs(Vector3Int pos, ChunkTile ctile) {
			var index = prefabInstance.BinarySearch(pos);
			if(index >= 0) {
				DraftsUtil.DestroyHybrid(prefabInstance[index].go);
				prefabInstance.RemoveAt(index);
			}
			if(ctile?.tile is PrefabTile pt) Instantiate(ctile);
		}

		public int CompareTo(ChunkView other) => chunk.Key.CompareTo(other.chunk.Key);
		public int CompareTo(Chunk other) => chunk.Key.CompareTo(other.Key);

		[ContextMenu("Redraw")] public void Redraw() => chunk.Redraw();

		[ContextMenu("Refresh Objects")]
		public void RefreshObjects() {
			prefabInstance.RemoveAll(pt => {
				DraftsUtil.DestroyHybrid(pt.go);
				return true;
			});

			foreach(var tile in Chunk)
				if(tile.Base is PrefabTile pt)
					Instantiate(tile);
		}

		void Instantiate(ChunkTile tile) {
			var pt = tile.Base as PrefabTile;
			var p = transform.position + Vector3.Scale(tile.position, Chunk.Manager.TileSize);
			var go = pt.Instantiate(p, Quaternion.identity, transform);
			prefabInstance.BinaryInsert(new() { tile = tile, go = go });
		}
	}
}
