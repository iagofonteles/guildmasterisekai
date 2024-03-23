using System.Linq;
using UnityEngine;
using System;
using Drafts.Tilemap3D.Generators.Paths;
namespace Drafts.Tilemap3D.Generators {
	public partial class PathTileGenerator : TileGenerator {

		protected override void Generate() {
			var Base = baseMeshes.Length < 4 ? CreateCorners(tileSize, Vector2.one / textureFrames / 2) : baseMeshes;

			for(int c = 0; c < Corners.Count; c++)
				for(int r = 0; r < rules.Length; r++)
					try {
						GenerateCorner(GetMesh(c, r), Base[c], rules[r]);
					} catch(Exception e) {
						Debug.LogError($"Error on step {c} - {r}");
						Debug.LogException(e);
					}
		}

		void GenerateCorner(Mesh mesh, Mesh _base, RuleConfig rule) {
			mesh.Clear();
			mesh.vertices = _base.vertices;
			mesh.normals = _base.normals;
			mesh.bounds = _base.bounds;
			mesh.tangents = _base.tangents;
			mesh.SetIndices(_base.GetIndices(0), MeshTopology.Quads, 0);

			var offset = new Vector2(
				(rule.frame % textureFrames.x) / (float)textureFrames.x,
				(rule.frame / textureFrames.x) / (float)textureFrames.y
			);

			if(rule.frame == -1) mesh.uv = _base.uv.Select(v => defaultFrameUV).ToArray();
			else mesh.uv = _base.uv.Select(v => v + offset).ToArray();
		}
	}
}
