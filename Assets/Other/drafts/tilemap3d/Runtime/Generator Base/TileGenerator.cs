using Drafts.Editing;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Drafts.Tilemap3D {

	public abstract partial class TileGenerator : ScriptableObject {
		public static Action OnChanged { get; internal set; }

		[SerializeField] protected List<Mesh> generated;
		[SerializeField] protected List<Mesh> optimization;
		[NonSerialized] int[] maskRuleMap;

		public abstract CornerConfig Corners { get; }
		public abstract IReadOnlyList<RuleSetup> Rules { get; }
		public virtual void Optimize(int[] cornersRules) { }
		protected abstract void Generate();

		public Mesh this[int index] => index switch {
			-1 => null,
			< -1 => optimization[-index - 2],
			_ => generated[index]
		};

		public Mesh GetMesh(int corner, int rule) => rule switch {
			-1 => null,
			< -1 => optimization[-rule - 2],
			_ => generated[corner * Rules.Count + rule]
		};

		public virtual byte[] GetConnections(ChunkTile tile) {
			var connections = new byte[Corners.Directions.Length];
			for(int i = 0; i < connections.Length; i++) {
				var pos = tile.position + Corners.Directions[i];
				connections[i] = (byte)(GetConnection(tile, pos) ? 1 : 0);
			}
			return connections;
		}

		protected virtual bool GetConnection(ChunkTile tile, Vector3Int pos) {
			var other = tile.chunk.GetTileRelative(pos, out _);
			return ((ITileInstance)tile).Base.ConnectTo(other?.Base);
		}

		public int[] GetCornersRule(byte[] connections) {
			maskRuleMap ??= CreateMaskRuleMap();
			var result = new int[Corners.Count];
			for(int i = 0; i < result.Length; i++)
				result[i] = maskRuleMap[Corners.GetMask(i, connections)];
			return result;
		}

		int[] CreateMaskRuleMap() {
			var result = new int[1 << Corners.Connections];
			for(int i = 0; i < result.Length; i++)
				result[i] = MapMaskToRule(i);
			return result;
		}

		int MapMaskToRule(int mask) {
			for(int i = 0; i < Rules.Count; i++)
				if(Rules[i].Match(mask)) return i;
			Debug.LogError($"mask {mask} doesnt match any rule!", this);
			return -1;
		}

	}
}