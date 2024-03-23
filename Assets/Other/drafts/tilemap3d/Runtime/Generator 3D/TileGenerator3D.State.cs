using System;
using System.Linq;
using UnityEngine;
namespace Drafts.Tilemap3D.Generators {

	public partial class TileGenerator3D {

		[Serializable]
		class OptimizationState {
			public int optMeshIndex;
			public int rule;
			public int[] corners;
		}
		[SerializeField] OptimizationState[] optimize;

		public override void Optimize(int[] cornersState) {
			foreach(var s in optimize)
				if(s.corners.All(i => cornersState[i] == s.rule)) {
					foreach(int i in s.corners) cornersState[i] = -1;
					cornersState[s.corners[0]] = s.optMeshIndex;
				}
		}
	}
}
