using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drafts.Extensions {
	public static class ExtensionsLerp {

		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this Transform[] t, float value) => value >= t.Length - 1 ? t[t.Length - 1].position : Vector3.Lerp(t[(int)value].position, t[(int)value + 1].position, value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this List<Transform> t, float value) => value >= t.Count - 1 ? t[t.Count - 1].position : Vector3.Lerp(t[(int)value].position, t[(int)value + 1].position, value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this Vector3[] t, float value) => value >= t.Length - 1 ? t[t.Length - 1] : Vector3.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this List<Vector3> t, float value) => value >= t.Count - 1 ? t[t.Count - 1] : Vector3.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this Vector2[] t, float value) => value >= t.Length - 1 ? t[t.Length - 1] : Vector2.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this List<Vector2> t, float value) => value >= t.Count - 1 ? t[t.Count - 1] : Vector2.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static float Lerp(this float[] t, float value) => value >= t.Length - 1 ? t[t.Length - 1] : Mathf.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static float Lerp(this List<float> t, float value) => value >= t.Count - 1 ? t[t.Count - 1] : Mathf.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static float Lerp(this int[] t, float value) => value >= t.Length - 1 ? t[t.Length - 1] : Mathf.Lerp(t[(int)value], t[(int)value + 1], value % 1);
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static float Lerp(this List<int> t, float value) => value >= t.Count - 1 ? t[t.Count - 1] : Mathf.Lerp(t[(int)value], t[(int)value + 1], value % 1);

		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static Vector3 Lerp(this IEnumerable<Vector3> t, float value) {
			var range = t.Skip((int)value).Take(2);
			return Vector3.Lerp(t.First(), t.Last(), value % 1);
		}
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static float Lerp(this IEnumerable<float> t, float value) {
			var range = t.Skip((int)value).Take(2);
			return Mathf.Lerp(t.First(), t.Last(), value % 1);
		}
		/// <summary>return a lerp from  the two adjacent indexes of value with t = value % 1.</summary>
		public static float Lerp(this IEnumerable<int> t, float value) {
			var range = t.Skip((int)value).Take(2);
			return Mathf.Lerp(t.First(), t.Last(), value % 1);
		}

	}
}
