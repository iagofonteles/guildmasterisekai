using Drafts.Editing;
using System;
using System.Linq;
using UnityEngine;

namespace Drafts.Tilemap3D {
	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/Tile")]
	public class Tile : ScriptableObject, ITile {
		[SerializeField] Texture icon;
		[SerializeField] TileGenerator generator;
		[SerializeField] Material[] materials;
		[SerializeField] TileGenerator[] connectTo;

		public TileGenerator Generator => generator;
		public Material[] Materials => materials;

		Texture ITile.Icon => icon;
		bool ITile.ConnectTo(ITile tile) => tile is Tile t && (t.generator == generator || connectTo.Contains(t.generator));
		ITileInstance ITile.Instantiate(Vector3 position) => throw new NotImplementedException();
	}
}