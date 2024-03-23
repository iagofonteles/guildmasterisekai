using System.Linq;
using UnityEngine;
using Drafts.Tilemap3D.Generators.Paths;
using UnityEditor;
using System.Collections.Generic;
namespace Drafts.Tilemap3D.Generators {

	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/Path Tile Generator")]
	public partial class PathTileGenerator : TileGenerator {

		[SerializeField] RuleConfig[] rules;
		[SerializeField] Mesh[] baseMeshes;
		[SerializeField] Vector2Int textureFrames = Vector2Int.one;
		[SerializeField] Vector3 tileSize = Vector3.one;
		[SerializeField] Vector2 defaultFrameUV;

		public override IReadOnlyList<RuleSetup> Rules => rules;
		public override CornerConfig Corners { get; } = new Corner();

		static Mesh[] CreateCorners(Vector3 size, Vector2 texQuad) {
			var result = new Mesh[4] { new(), new(), new(), new(), };
			var y = -.499f;
			var sv = size / 2;
			var verts = new Vector3[] { new(-sv.x, y, -sv.z), new(0, y, -sv.z), new(-sv.x, y, 0), new(0, y, 0) };
			var uvs = new Vector2[] { new(0, 0), new(texQuad.x, 0), new(0, texQuad.y), new(texQuad.x, texQuad.y) };

			result[0].SetVertices(verts.Select(v => v + new Vector3(0, 0, 0)).ToArray());
			result[1].SetVertices(verts.Select(v => v + new Vector3(sv.x, 0, 0)).ToArray());
			result[2].SetVertices(verts.Select(v => v + new Vector3(0, 0, sv.z)).ToArray());
			result[3].SetVertices(verts.Select(v => v + new Vector3(sv.x, 0, sv.z)).ToArray());

			result[0].SetUVs(0, uvs.Select(v => v + new Vector2(0, 0)).ToArray());
			result[1].SetUVs(0, uvs.Select(v => v + new Vector2(texQuad.x, 0)).ToArray());
			result[2].SetUVs(0, uvs.Select(v => v + new Vector2(0, texQuad.y)).ToArray());
			result[3].SetUVs(0, uvs.Select(v => v + new Vector2(texQuad.x, texQuad.y)).ToArray());

			foreach(var m in result) {
				m.SetIndices(new int[] { 0, 2, 3, 1 }, MeshTopology.Quads, 0);
				m.RecalculateBounds();
				m.RecalculateNormals();
				m.RecalculateTangents();
			}
			return result;
		}
	}
}
