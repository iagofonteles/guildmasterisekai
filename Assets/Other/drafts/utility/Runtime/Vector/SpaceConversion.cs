using UnityEngine;

namespace Drafts.Extensions {

	public static partial class ExtensionsVector {
		/// <summary>Uses main camera to get the world coordinates.</summary>
		/// <param name="distance">The distance from the main camera, needed for perspective camera.</param>
		public static Vector3 ToWorld(this Vector2 v, float distance = 0) => Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, distance));
		/// <summary>Uses main camera to get the world coordinates.</summary>
		public static Vector3 ToWorld(this Vector3 v) => Camera.main.ScreenToWorldPoint(v);
		/// <summary>Uses main camera to get the world coordinates.</summary>
		public static Vector3 ToWorld(this Vector3 v, float distance) => Camera.main.ScreenToWorldPoint(v.Z(distance));

		/// <summary>Uses main camera to get the screen coordinates.</summary>
		public static Vector2 ToScreen(this Vector2 v) => Camera.main.WorldToScreenPoint(v);
		/// <summary>Uses main camera to get the screen coordinates.</summary>
		public static Vector3 ToScreen(this Vector3 v) => Camera.main.WorldToScreenPoint(v);

		/// <summary>Uses main camera to get the world coordinates.</summary>
		public static Rect ToWorld(this Rect v) {
			var ret = new Rect();
			ret.min = v.min.ToWorld();
			ret.max = v.max.ToWorld();
			return ret;
		}
		/// <summary>Uses main camera to get the screen coordinates.</summary>
		public static Rect ToScreen(this Rect v) {
			var ret = new Rect();
			ret.min = v.min.ToScreen();
			ret.max = v.max.ToScreen();
			return ret;
		}
		/// <summary>Uses main camera to get the world coordinates.</summary>
		public static Bounds ToWorld(this Bounds v) {
			var ret = new Bounds();
			ret.min = v.min.ToWorld();
			ret.max = v.max.ToWorld();
			return ret;
		}
		/// <summary>Uses main camera to get the screen coordinates.</summary>
		public static Bounds ToScreen(this Bounds v) {
			var ret = new Bounds();
			ret.min = v.min.ToScreen();
			ret.max = v.max.ToScreen();
			return ret;
		}
		/// <summary>Uses main camera to get the world coordinates.</summary>
		public static Bounds WorldBounds(this RectTransform v) {
			var c = new Vector3[4];
			v.GetWorldCorners(c);
			var ret = new Bounds();
			ret.min = c[0];
			ret.max = c[2];
			return ret;
		}
		/// <summary>Uses main camera to get the world coordinates.</summary>
		public static Rect WorldRect(this RectTransform v) {
			var c = new Vector3[4];
			v.GetWorldCorners(c);
			var ret = new Rect();
			ret.min = c[0];
			ret.max = c[2];
			return ret;
		}

	}
}