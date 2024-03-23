using System;
using UnityEngine;
namespace Drafts.Tilemap3D.Generators.Corners {

	[Flags]
	public enum Neighbor { x = 1, y = 2, z = 4, xi = 8, yi = 16, zi = 32, d = 64, }
	public enum UvMapping { None, yFlip, Rotate, TopProj }

	[Serializable]
	public class RuleConfig : RuleSetup {
		public Neighbor conn;
		public Neighbor free;
		public int swap = 0;
		protected override int Conn => (int)conn;
		protected override int Free => (int)free;
	}

	[Serializable]
	public class MeshConfig {
		public Mesh mesh;
		public SubMeshConfig[] subMeshes;
	}

	[Serializable]
	public class SubMeshConfig {
		public bool use = true;
		public int texFrame;
		public UvMapping uvMapping;
	}
}
