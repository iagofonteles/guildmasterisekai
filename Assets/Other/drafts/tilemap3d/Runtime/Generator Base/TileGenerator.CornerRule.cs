using UnityEngine;
namespace Drafts.Tilemap3D {

	public abstract class RuleSetup {
		/// <summary>Mandatory free spaces.</summary>
		protected abstract int Free { get; }
		/// <summary>Mandatory connections.</summary>
		protected abstract int Conn { get; }
		public bool Match(int mask) {
			if((mask & Free) > 0) return false;
			return (mask & Conn) == Conn;
		}
	}

	public abstract class CornerConfig {
		/// <summary>Number of corners.</summary>
		public int Count => NeighborMap.Length;
		/// <summary>Number of connection per corner.</summary>
		public int Connections => NeighborMap[0].Length;
		/// <summary>All directions a tile need to check.</summary>
		public abstract Vector3Int[] Directions { get; }
		/// <summary>For each corner, wich directions are considered.</summary>
		protected abstract int[][] NeighborMap { get; }

		public int GetMask(int cornerId, byte[] connections) {
			var map = NeighborMap[cornerId];
			var mask = 0;
			for(int j = 0; j < map.Length; j++) mask |= connections[map[j]] << j;
			return mask;
		}
	}
}