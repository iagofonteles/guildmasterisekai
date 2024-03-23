using UnityEngine;

namespace Drafts {
	public static partial class VectorUtil {

		public static Vector2Int FloorToInt(this Vector2 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		public static Vector2Int RountToInt(this Vector2 v) => new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
		public static Vector2Int CeilToInt(this Vector2 v) => new(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

		public static Vector3Int CeilToInt(this Vector3 v) => new(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
		public static Vector3Int RountToInt(this Vector3 v) => new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
		public static Vector3Int FloorToInt(this Vector3 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

	}
}