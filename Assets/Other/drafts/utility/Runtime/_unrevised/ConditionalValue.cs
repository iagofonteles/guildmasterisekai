using System;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace Drafts.Patterns {

	[Serializable]
	public abstract class Conditional<B, T> {
		public B active;
		public T value;
		protected abstract bool Bool { get; }
		public static implicit operator bool(Conditional<B, T> v) => v.Bool;
		public static implicit operator T(Conditional<B, T> v) => v.value;
	}

	[Serializable]
	public class Conditional<T> : Conditional<bool, T> {
		protected override bool Bool => active;
		public Conditional() { }
		public Conditional(T value) {
			active = true;
			this.value = value;
		}
		public Conditional(bool active, T value) {
			this.active = active;
			this.value = value;
		}
		public static implicit operator bool(Conditional<T> v) => v.Bool;
		public static implicit operator T(Conditional<T> v) => v.value;
		public static implicit operator Conditional<T>(T v) => new Conditional<T>(v);
	}

	[Serializable]
	public abstract class ConditionalObj<T> : Conditional<UObj, T> {
		protected override bool Bool => active;
		public static implicit operator bool(ConditionalObj<T> v) => v.Bool;
		public static implicit operator T(ConditionalObj<T> v) => v.value;
	}

	[Serializable]
	public class ConditionalCurve : Conditional<AnimationCurve> {
		public float Evaluate(float value) => this ? this.value.Evaluate(value) : value;
	}
}