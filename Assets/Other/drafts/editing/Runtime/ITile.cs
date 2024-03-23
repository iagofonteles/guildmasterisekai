using UnityEngine;
namespace Drafts.Editing {

	public interface ITile {
		string name { get; }
		Texture Icon { get; }
		bool ConnectTo(ITile prefab);
		ITileInstance Instantiate(Vector3 position);
	}

	public interface ITileInstance {
		ITile Base { get; }
		IChunk Chunk { get; }
		Vector3Int Position { get; }
		Vector3 WorldPosition { get; }

		sealed Vector3 GetWorldPosition() => Vector3.Scale(Chunk.Manager.TileSize,
			 Chunk.Key * Chunk.Manager.ChunkLength + Position);

		void OnPlaced(IChunk chunk, Vector3Int pos);
		void OnRemoved(IChunk chunk, Vector3Int pos);
		//sealed void Place(ITile @base, IChunk chunk, Vector3Int pos) {
		//	Base = @base;
		//	Chunk = chunk;
		//	Position = pos;
		//	OnPlaced(chunk, pos);
		//}
	}

}
