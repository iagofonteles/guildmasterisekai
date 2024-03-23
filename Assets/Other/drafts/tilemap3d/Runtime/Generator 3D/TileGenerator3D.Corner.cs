using UnityEngine;
namespace Drafts.Tilemap3D.Generators.Corners {
	public class Corner : CornerConfig {

		public override Vector3Int[] Directions { get; } = new Vector3Int[] {
			Vector3Int.left,	// 0
			Vector3Int.right,	// 1
			Vector3Int.down,	// 2
			Vector3Int.up,		// 3
			Vector3Int.back,	// 4
			Vector3Int.forward,	// 5

			Vector3Int.left + Vector3Int.back,		// 6
			Vector3Int.left + Vector3Int.forward,	// 7
			Vector3Int.right + Vector3Int.back,		// 8
			Vector3Int.right + Vector3Int.forward,	// 9

			Vector3Int.down + Vector3Int.left,		// 10
			Vector3Int.down + Vector3Int.right,		// 11
			Vector3Int.down + Vector3Int.back,		// 12
			Vector3Int.down + Vector3Int.forward,	// 13

			Vector3Int.up + Vector3Int.left,		// 14
			Vector3Int.up + Vector3Int.right,		// 15
			Vector3Int.up + Vector3Int.back,		// 16
			Vector3Int.up + Vector3Int.forward,		// 17

			Vector3Int.left + Vector3Int.down + Vector3Int.back,	// 18
			Vector3Int.right + Vector3Int.down + Vector3Int.back,	// 19
			Vector3Int.left + Vector3Int.up + Vector3Int.back,		// 20
			Vector3Int.right + Vector3Int.up + Vector3Int.back,		// 21
			Vector3Int.left + Vector3Int.down + Vector3Int.forward,	// 22
			Vector3Int.right + Vector3Int.down + Vector3Int.forward,// 23
			Vector3Int.left + Vector3Int.up + Vector3Int.forward,	// 24
			Vector3Int.right + Vector3Int.up + Vector3Int.forward,	// 25
		};

		protected override int[][] NeighborMap { get; } = new int[8][] {
			new int[7] { 0, 2, 4,  12, 6, 10,  18 },
			new int[7] { 1, 2, 4,  12, 8, 11,  19 },
			new int[7] { 0, 3, 4,  16, 6, 14,  20 },
			new int[7] { 1, 3, 4,  16, 8, 15,  21 },
			new int[7] { 0, 2, 5,  13, 7, 10,  22 },
			new int[7] { 1, 2, 5,  13, 9, 11,  23 },
			new int[7] { 0, 3, 5,  17, 7, 14,  24 },
			new int[7] { 1, 3, 5,  17, 9, 15,  25 },
		};
	}
}
