using Drafts.Editing;
using System.Collections.Generic;
using UnityEngine;
namespace Drafts.Tilemap3D {

	[SelectionBase, ExecuteAlways]
	public class TilemapView : ChunkManagerView {
		[SerializeField] Tilemap map;
		[SerializeField] List<ChunkView> chunks;

		public override IChunkManager ChunkManager => map;

		public override void Subscribe() {
			if(!map) return;
			map.OnChunkAdded -= OnChunkAdded;
			map.OnChunkAdded += OnChunkAdded;
			map.OnChunkRemoved -= OnChunkRemoved;
			map.OnChunkRemoved += OnChunkRemoved;
			foreach(var chunk in chunks) chunk.Subscribe();
		}

		public override void Unsubscribe() {
			if(!map) return;
			map.OnChunkAdded -= OnChunkAdded;
			map.OnChunkRemoved -= OnChunkRemoved;
			foreach(var chunk in chunks) chunk.Unsubscribe();
		}

		private void OnChunkAdded(Chunk chunk) {
			var go = new GameObject("chunk " + chunk.Key);
			go.transform.parent = transform;
			go.transform.position = Vector3.Scale(Vector3.Scale(chunk.Key, map.ChunkLength), map.TileSize);
			var view = go.AddComponent<ChunkView>();
			view.Chunk = chunk;
			chunks.BinaryInsert(view);
		}

		private void OnChunkRemoved(Chunk obj) {
			Debug.Log("removed");
			var c = chunks.BinaryFind(obj);
			if(c) DraftsUtil.DestroyHybrid(c.gameObject);
		}

		[ContextMenu("Refresh")]
		public void Refresh() {
			if(!map) return;

			foreach(var chunk in map) {
				var index = chunks.BinarySearch(chunk);
				if(index < 0) OnChunkAdded(chunk);
				else chunks[index].Refresh();
			}
			chunks.RemoveAll(c => {
				if(c.Chunk) return false;
				DraftsUtil.DestroyHybrid(c.gameObject);
				return true;
			});
		}

		[ContextMenu("Redraw")] public void Redraw() => map.Redraw();
		[ContextMenu("Recalculate")] public void Recalculate() => map.Recalculate();
		[ContextMenu("Refresh Objects")] public void RefreshObjects() { foreach(var chunk in chunks) chunk.RefreshObjects(); }
	}
}