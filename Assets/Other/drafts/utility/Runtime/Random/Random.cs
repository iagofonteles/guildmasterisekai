using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URand = UnityEngine.Random;

namespace Drafts {

	/// <summary>Various random generated numbers and points.</summary>
	public static class DRandom {

		/// <summary>Return a random opaque color.</summary>
		public static Color Color => new Color(URand.value, URand.value, URand.value);
		/// <summary>Return a random color with random opacity.</summary>
		public static Color ColorAlpha => new Color(URand.value, URand.value, URand.value, URand.value);
		/// <summary>Return true or false.</summary>
		public static bool Bool => URand.value > .5;
		/// <summary>Return 1 or -1.</summary>
		public static int Side => URand.value > 0.5f ? 1 : -1;
		/// <summary>Return a random normalized directionin 3d space.</summary>
		public static Vector3 Direction => URand.onUnitSphere;
		/// <summary>Return a random normalized direction in 2d space.</summary>
		public static Vector2 Direction2D => URand.insideUnitCircle.normalized;
		/// <summary>Return a point between 1 and -1 on all dimensions.</summary>
		public static Vector3 InsideUnitCube => new Vector3(URand.Range(-1f, 1f), URand.Range(-1f, 1f), URand.Range(-1f, 1f));
		/// <summary>Return a point between 1 and -1 on all dimensions.</summary>
		public static Vector2 InsideUnitSquare => new Vector2(URand.Range(-1f, 1f), URand.Range(-1f, 1f));

		/// <summary>Return a point inside the collider.</summary>
		public static Vector3 Inside(SphereCollider col) => col.transform.position + col.center + URand.insideUnitSphere * col.radius;
		/// <summary>Return a point inside the collider.</summary>
		public static Vector3 Inside(BoxCollider col) => col.transform.position + col.center + col.transform.rotation * Vector3.Scale(InsideUnitCube, col.size / 2);
		/// <summary>Return a point inside the collider.</summary>
		public static Vector2 Inside(CircleCollider2D col) => (Vector2)col.transform.position + col.offset + URand.insideUnitCircle * col.radius;
		/// <summary>Return a point inside the collider.</summary>
		public static Vector2 Inside(BoxCollider2D col) => (Vector2)col.transform.position + col.offset + Vector2.Scale(InsideUnitSquare, col.size / 2);

		/// <summary>Return a point inside the bounds.</summary>
		public static Vector2 Inside(Vector2 min, Vector2 max) => new Vector2(URand.Range(min.x, max.x), URand.Range(min.y, max.y));
		/// <summary>Return a point inside the bounds.</summary>
		public static Vector3 Inside(Vector3 min, Vector3 max) => new Vector3(URand.Range(min.x, max.x), URand.Range(min.y, max.y), URand.Range(min.z, max.z));
		/// <summary>Return a point inside a rect.</summary>
		public static Vector2 Inside(Rect rect) => Inside(rect.min, rect.max);
		/// <summary>Return a point inside bounds.</summary>
		public static Vector3 Inside(Bounds bounds) => Inside(bounds.min, bounds.max);

		public static T From<T>(params T[] values) => values[URand.Range(0, values.Length)];
		public static T From<T>(List<T> values) => values[URand.Range(0, values.Count)];
		public static T From<T>(IEnumerable<T> values) => values.Skip(URand.Range(0, values.Count())).First();

		[Obsolete("Not implemented", true)]
		public static T Wheel<T>(T[] values) => values[URand.Range(0, values.Length)];

		public static float Variation(this float value, float percent) => value + value * URand.Range(-percent, percent);
		public static float VariationSigned(this float value, float percent) => Variation(value, percent) * Side;
		public static int Max(int max) => URand.Range(0, max);
	}
}