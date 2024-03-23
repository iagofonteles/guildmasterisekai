using UnityEngine;
using System;
using UnityEngine.Rendering;
using Drafts.Tilemap3D.Generators.Corners;
using System.Collections.Generic;

namespace Drafts.Tilemap3D.Generators {

	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/Corner Tile Generator")]
	public partial class TileGenerator3D : TileGenerator {

		[SerializeField, NonReorderable] RuleConfig[] rules;
		[SerializeField, NonReorderable] MeshConfig[] topMeshes;
		[SerializeField, NonReorderable] MeshConfig[] botMeshes;

		[Header("Result")]
		[SerializeField] int mainTexFrames = 4;

		public override IReadOnlyList<RuleSetup> Rules => rules;
		public override CornerConfig Corners { get; } = new Corner();

		private void OnValidate() {
			if(topMeshes.Length != rules.Length) Array.Resize(ref topMeshes, rules.Length);
			if(botMeshes.Length != rules.Length) Array.Resize(ref botMeshes, rules.Length);
		}

		void SetUvQuad(Vector2[] uvs, SubMeshDescriptor subMesh, int quad) {
			for(int i = subMesh.firstVertex; i < subMesh.firstVertex + subMesh.vertexCount; i++)
				uvs[i] = SetUvQuad(uvs[i], quad);
		}

		Vector2 SetUvQuad(Vector2 uv, int quad) {
			uv.x = uv.x / mainTexFrames + 1 / mainTexFrames * quad;
			return uv;
		}

		static void Flip(Vector2[] uvs, int index, int count) {
			for(int i = index; i < index + count; i++)
				uvs[i] = new(1 - uvs[i].x, 1 - uvs[i].y);
		}

		static Vector2[] UVbyPositionByCondition(Vector2[] uvs, Vector3[] vertices, Func<int, bool> checkVertex, bool flip) {
			var ret = new Vector2[vertices.Length];
			for(int i = 0; i < vertices.Length; i++) {
				ret[i] = checkVertex(i) ? new(vertices[i].x + .5f, vertices[i].z + .5f) : uvs[i];
				if(flip) ret[i] = Vector2.one - ret[i];
			}
			return ret;
		}

	}
}