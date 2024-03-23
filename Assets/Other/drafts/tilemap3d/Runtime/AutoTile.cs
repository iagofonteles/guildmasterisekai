using Drafts.Editing;
using System;
using System.Linq;
using UnityEngine;

namespace Drafts.Tilemap3D {
	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/AutoTile")]
	public class AutoTile : Tile {
		[SerializeField] TileGenerator generator;
		[SerializeField] Material[] materials;
		[SerializeField] TileGenerator[] connectTo;

		public TileGenerator Generator => generator;
		public Material[] Materials => materials;

		public bool ConnectTo(ITile tile) => tile is AutoTile t && (t.generator == generator || connectTo.Contains(t.generator));
	}
}