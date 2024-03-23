using UnityEngine;

namespace Drafts {
	public static partial class VectorUtil {

		public static int CompareTo(this Vector3 a, Vector3 b) {
			var ret = a.x.CompareTo(b.x);
			if(ret != 0) return ret;

			ret = a.y.CompareTo(b.y);
			if(ret != 0) return ret;

			return a.z.CompareTo(b.z);
		}

		public static int CompareTo(this Vector3Int a, Vector3Int b) {
			var ret = a.x.CompareTo(b.x);
			if(ret != 0) return ret;

			ret = a.y.CompareTo(b.y);
			if(ret != 0) return ret;

			return a.z.CompareTo(b.z);
		}

	}
}