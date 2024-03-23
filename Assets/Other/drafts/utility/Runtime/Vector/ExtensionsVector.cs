using UnityEngine;

namespace Drafts.Extensions {

	public static partial class ExtensionMethods {

		/// <summary>Return (a - b).sqrMagnitude < .001f.</summary>
		public static bool Near(this Vector3 a, Vector3 b) => (a - b).sqrMagnitude < .001f;

		// Vector3
		public static Vector3 Change(this Vector3 v, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
		public static Vector3Int Change(this Vector3Int v, int? x = null, int? y = null, int? z = null) => new Vector3Int(x ?? v.x, y ?? v.y, z ?? v.z);

		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector3 X(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector3 Y(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector3 Z(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector2 X(this Vector2 v, float x) => new Vector2(x, v.y);
		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector2 Y(this Vector2 v, float y) => new Vector2(v.x, y);

		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector3Int X(this Vector3Int v, int x) => new Vector3Int(x, v.y, v.z);
		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector3Int Y(this Vector3Int v, int y) => new Vector3Int(v.x, y, v.z);
		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector3Int Z(this Vector3Int v, int z) => new Vector3Int(v.x, v.y, z);

		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector2Int X(this Vector2Int v, int x) => new Vector2Int(x, v.y);
		/// <summary>Return a new vector with one coordinate changed.</summary>
		public static Vector2Int Y(this Vector2Int v, int y) => new Vector2Int(v.x, y);

		/// <summary>Return a new vector that is floored on each individual axis</summary>
		public static Vector3 Floored(this Vector3 v) => new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector3 Rounded(this Vector3 v) => new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector3 Ceiled(this Vector3 v) => new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));

		/// <summary>Return a new int vector that is floored on each individual axis</summary>
		public static Vector3Int FloorToInt(this Vector3 v) => new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector3Int RoundToInt(this Vector3 v) => new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector3Int CeilToInt(this Vector3 v) => new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));

		/// <summary>Return a new vector that is floored on each individual axis</summary>
		public static Vector2 Floored(this Vector2 v) => new Vector2((int)v.x, (int)v.y);
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector2 Rounded(this Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector2 Ceiled(this Vector2 v) => new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));

		/// <summary>Return a new int vector that is floored on each individual axis</summary>
		public static Vector2Int FloorToInt(this Vector2 v) => new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector2Int RoundToInt(this Vector2 v) => new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
		/// <summary>Return a new vector that is rounded on each individual axis</summary>
		public static Vector2Int CeilToInt(this Vector2 v) => new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

		/// <summary>Return a new vector that is aligned with a grid o size [gridSize]</summary>
		public static Vector2 Aligned(this Vector2 v, Vector2 gridSize) => new Vector2(v.x - v.x % gridSize.x, v.y - v.y % gridSize.y);
		/// <summary>Return a new vector that is aligned with a grid o size [gridSize]</summary>
		public static Vector3 Aligned(this Vector3 v, Vector3 gridSize) => new Vector3(v.x - v.x % gridSize.x, v.y - v.y % gridSize.y, v.z - v.z % gridSize.z);

		/// <summary>Modify this vector and return true if reached target value using MoveTowards.</summary>
		public static bool Reach(this ref Vector2 vector, Vector2 target, float speed) {
			vector = Vector2.MoveTowards(vector, target, speed);
			return vector == target;
		}
		/// <summary>Modify this vector and return true if reached target value using MoveTowards.</summary>
		public static bool Reach(this ref Vector3 vector, Vector3 target, float speed) {
			vector = Vector3.MoveTowards(vector, target, speed);
			return vector == target;
		}
	}
}
