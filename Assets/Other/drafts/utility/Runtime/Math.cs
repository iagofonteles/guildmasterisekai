using UnityEngine;
namespace Drafts {
	public static partial class DMath {

		public static int Loop(this int n, int min, int max, out int times) {
			int range = max - min;
			times = Mathf.FloorToInt((n - min) / (float)range);
			return n - times * range;
		}

		public static float Loop(this float n, float min, float max, out int times) {
			float range = max - min;
			times = Mathf.FloorToInt((n - min) / range);
			return n - times * range;
		}

	}
}