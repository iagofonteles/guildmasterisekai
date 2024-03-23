using System;
using UnityEngine;

namespace Drafts.Tilemap3D {

	[Obsolete("",true)]
	public class CubeFaces {

		/// <summary>
		/// left, right, down, up, back, forw
		/// </summary>
		public Mesh[] faces = new Mesh[6];
		public Mesh this[int face] => faces[face];

		public CubeFaces(float size) {
			var half = size / 2f;

			for(int i = 0; i < faces.Length; i++)
				faces[i] = new Mesh();

			faces[0].SetVertices(new Vector3[] {
				new(-half, -half, +half),
				new(-half, +half, +half),
				new(-half, -half, -half),
				new(-half, +half, -half),
			});
			faces[0].SetTriangles(new int[] { 0, 1, 2, 1, 3, 2 }, 0);

			faces[1].SetVertices(new Vector3[] {
				new(+half, -half, -half),
				new(+half, +half, -half),
				new(+half, -half, +half),
				new(+half, +half, +half),
			});
			faces[1].SetTriangles(new int[] { 0, 1, 2, 1, 3, 2 }, 0);

			faces[2].SetVertices(new Vector3[] {
				new(-half, -half, +half),
				new(-half, -half, -half),
				new(+half, -half, +half),
				new(+half, -half, -half),
			});
			faces[2].SetTriangles(new int[] { 0, 1, 2, 1, 3, 2 }, 0);

			faces[3].SetVertices(new Vector3[] {
				new(-half, +half, -half),
				new(-half, +half, +half),
				new(+half, +half, -half),
				new(+half, +half, +half),
			});
			faces[3].subMeshCount = 2;
			faces[3].SetTriangles(new int[] { 0, 1, 2, 1, 3, 2 }, 1);

			faces[4].SetVertices(new Vector3[] {
				new(-half, -half, -half),
				new(-half, +half, -half),
				new(+half, -half, -half),
				new(+half, +half, -half),
			});
			faces[4].SetTriangles(new int[] { 0, 1, 2, 1, 3, 2 }, 0);

			faces[5].SetVertices(new Vector3[] {
				new(+half, -half, +half),
				new(+half, +half, +half),
				new(-half, -half, +half),
				new(-half, +half, +half),
			});
			faces[5].SetTriangles(new int[] { 0, 1, 2, 1, 3, 2 }, 0);

			foreach(var face in faces) {
				face.SetUVs(0, new Vector2[] { new(0, 0), new(0, 1), new(1, 0), new(1, 1), });
				face.RecalculateNormals();
			}

		}
	}
}
