using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Drafts.Patterns {

	/// <summary>
	/// A Monobehaviour Multiton Solution
	/// Store every created instance on Awake and remove on OnDestroy.
	/// </summary>
	public abstract class MonoMultiton<T> : MonoBehaviour where T : MonoMultiton<T> {
		/// <summary>All created instances.</summary>
		public static List<T> Instances { get; } = new List<T>();
		protected virtual void Awake() => Instances.Add((T)this);
		protected virtual void OnDestroy() => Instances.Remove((T)this);
		public static T First(Func<T, bool> predicate) => Instances.FirstOrDefault(predicate);
		public static T Find(string name) => Instances.FirstOrDefault(i => i.name == name);
		public static T Nearest(Vector3 position) => Instances.OrderBy(i => (i.transform.position - position).sqrMagnitude).FirstOrDefault();
	}

}
