using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Drafts.Tilemap3D.Generators.Corners;
namespace Drafts.Tilemap3D.Generators {
	public partial class TileGenerator3D {

		static Quaternion[] rotations; // rotate the model to form the other corners (per corner)
		static bool[] vFlips; // flips the uv and submesh for top/bot (per corner)
		static bool[] swapXZ; // use the base model for XYZ instead of ZYX (per corner)

		static TileGenerator3D() {
			vFlips = new bool[8] { true, true, false, false, true, true, false, false };
			swapXZ = new bool[8] { true, false, false, true, false, true, true, false };
			rotations = new Quaternion[8] {
				VectorUtil.Get90DegRotation(new(0, 3, 2)), //0
				VectorUtil.Get90DegRotation(new(2, 0, 0)), //1
				VectorUtil.Get90DegRotation(new(0, 2, 0)), //2
				VectorUtil.Get90DegRotation(new(0, 1, 0)), //3
				VectorUtil.Get90DegRotation(new(0, 0, 2)), //4
				VectorUtil.Get90DegRotation(new(0, 1, 2)), //5
				VectorUtil.Get90DegRotation(new(0, 3, 0)), //6
				VectorUtil.Get90DegRotation(new(0, 0, 0)), //7
			};
		}

		protected override void Generate() {
			for(int i = 0; i < Corners.Count; i++) {
				var meshes = (i & 2) > 0 ? topMeshes : botMeshes;
				GenerateCorner(generated, i, meshes, rotations[i], vFlips[i], swapXZ[i]);
			}
		}

		void GenerateCorner(List<Mesh> meshes, int corner, MeshConfig[] config, Quaternion rotation, bool vFlip, bool swapBits) {
			for(int i = 0; i < config.Length; i++) {
				try {
					var swap = rules[i].swap;
					var baseId = i + (swapBits ? swap : 0);
					var _base = config[baseId].mesh;
					var mesh = meshes[corner * rules.Length + i];

					mesh.Clear();
					if(!_base) continue;
					mesh.vertices = _base.vertices.Rotate(rotation);
					mesh.normals = _base.normals.Rotate(rotation);
					var uvs = _base.uv;

					var min = config[i].subMeshes.Min(c => c.texFrame);
					if(min < 0) mesh.subMeshCount = -min + 1;

					var tris = new List<List<int>>();
					for(int j = 0; j < mesh.subMeshCount; j++) tris.Add(new());

					for(int j = 0; j < _base.subMeshCount; j++) {
						if(j >= config[i].subMeshes.Length) continue;
						if(!config[i].subMeshes[j].use) continue;

						var sub = _base.GetSubMesh(j);
						var conf = config[i].subMeshes[j];

						if(conf.texFrame >= 0) SetUvQuad(uvs, sub, conf.texFrame);

						switch(conf.uvMapping) {
							case UvMapping.yFlip: if(vFlip) Flip(uvs, sub.firstVertex, sub.vertexCount); break;
							case UvMapping.TopProj: break;
							case UvMapping.Rotate: break;
							default: break;
						}

						var subMeshIndex = conf.texFrame < 0 ? -conf.texFrame : 0;
						tris[subMeshIndex].AddRange(_base.GetTriangles(j));
					}

					for(int j = 0; j < mesh.subMeshCount; j++) mesh.SetTriangles(tris[j], j);
					mesh.uv = uvs;

				} catch(Exception e) {
					Debug.LogError($"Error on step {i}");
					Debug.LogException(e);
				}
			}
		}

	}
}