using UnityEngine;

namespace Drafts {
	public static partial class VectorUtil {

		/// <summary>Faster rotation. Does not use trigonometrics.</summary>
		public static Vector2[] Rotate90Deg(Vector2[] points, Vector2 pivot, int times) {
			var ret = new Vector2[points.Length];
			for(int i = 0; i < points.Length; i++)
				ret[i] = Rotate90Deg(points[i], pivot, times);
			return ret;
		}

		/// <summary>Faster rotation. Does not use trigonometrics.</summary>
		public static Vector2 Rotate90Deg(Vector2 point, Vector2 pivot, int times) {
			point -= pivot;
			point = Rotate90Deg(point, times);
			point += pivot;
			return point;
		}

		/// <summary>Faster rotation. Does not use trigonometrics.</summary>
		public static Vector2 Rotate90Deg(Vector2 point, int times)
			=> (times % 4) switch {
				1 => new Vector2(point.y, -point.x),
				2 => -point,
				3 => new Vector2(-point.y, point.x),
				_ => point,
			};

		public static Quaternion Get90DegRotation(Vector3Int times) {
			var rotation = Quaternion.identity;
			if(times.x != 0) rotation *= Quaternion.AngleAxis(times.x * 90f, Vector3.right);
			if(times.y != 0) rotation *= Quaternion.AngleAxis(times.y * 90f, Vector3.up);
			if(times.z != 0) rotation *= Quaternion.AngleAxis(times.z * 90f, Vector3.forward);
			return rotation;
		}

		public static Vector3[] Rotate(this Vector3[] points, Quaternion rotation) {
			var ret = new Vector3[points.Length];
			for(int i = 0; i < points.Length; i++) ret[i] = rotation * points[i];
			return ret;
		}

		public static Vector3[] Rotate90Deg(Vector3[] points, Vector3Int times) {
			var rotation = Get90DegRotation(times);
			var ret = new Vector3[points.Length];
			for(int i = 0; i < points.Length; i++) ret[i] = rotation * points[i];
			return ret;
		}

		public static Vector3[] Transform(this Vector3[] points, Matrix4x4 transform) {
			var ret = new Vector3[points.Length];
			for(int i = 0; i < points.Length; i++) ret[i] = transform.MultiplyPoint3x4(points[i]);
			return ret;
		}

		public static Vector3[] Scale(this Vector3[] points, Vector3 scale) {
			var ret = new Vector3[points.Length];
			for(int i = 0; i < points.Length; i++) ret[i] = Vector3.Scale(points[i], scale);
			return ret;
		}

	}
}