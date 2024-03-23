using System.Collections.Generic;
using UnityEngine;

namespace Drafts {
	public static partial class ExtensionsTransform {

		public static List<T> GetComponentsImmediate<T>(this Transform transform) {
			var result = new List<T>();
			foreach(Transform t in transform)
				if(t.TryGetComponent<T>(out var c)) result.Add(c);
			return result;
		}

		public static List<T> GetComponentsImmediate<T>(this GameObject go)
			=> GetComponentsImmediate<T>(go.transform);
	}
}