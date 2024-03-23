using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drafts {
	public static class MeshUtil {
		public static void CombineSubmeshes<T>(Mesh target, IEnumerable<T> source, Func<T, Mesh> getMesh, Func<T, Vector3> getTransform) {
			var verts = new List<Vector3>();
			var norms = new List<Vector3>();
			var uvs = new List<Vector2>();
			var tris = new List<List<int>>();

			foreach(var s in source) {
				var mesh = getMesh(s);
				var pos = getTransform(s);
				if(!mesh) continue;

				var first = verts.Count;
				verts.AddRange(mesh.vertices.Select(v => v + pos));
				norms.AddRange(mesh.normals);
				uvs.AddRange(mesh.uv);

				for(int i = 0; i < mesh.subMeshCount; i++) {
					if(tris.Count <= i) tris.Add(new());
					tris[i].AddRange(mesh.GetTriangles(i).Select(i => i + first));
				}
			}

			target.Clear();
			target.SetVertices(verts);
			target.SetNormals(norms);
			target.SetUVs(0, uvs);

			target.subMeshCount = tris.Count;
			for(int i = 0; i < tris.Count; i++)
				target.SetTriangles(tris[i], i);
		}

		public static void CombineSubmeshes(Mesh target, IEnumerable<(Mesh mesh, Vector3 transform)> source)
			=> CombineSubmeshes(target, source, s => s.mesh, s => s.transform);
	}
}
