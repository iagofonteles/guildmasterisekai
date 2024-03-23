using UnityEngine;
namespace Drafts.Tilemap3D.Generators.Paths {
	public class Corner : CornerConfig {

		public override Vector3Int[] Directions { get; } = new Vector3Int[] {
			Vector3Int.left,	// 0
			Vector3Int.right,	// 1
			Vector3Int.back,	// 2
			Vector3Int.forward,	// 3
			Vector3Int.left + Vector3Int.back,		// 4
			Vector3Int.left + Vector3Int.forward,	// 5
			Vector3Int.right + Vector3Int.back,		// 6
			Vector3Int.right + Vector3Int.forward,	// 7
		};

		protected override int[][] NeighborMap { get; } = new int[4][] {
			new int[3] { 0, 2, 4 },
			new int[3] { 1, 2, 6 },
			new int[3] { 0, 3, 5 },
			new int[3] { 1, 3, 7 },
		};
	}
}
