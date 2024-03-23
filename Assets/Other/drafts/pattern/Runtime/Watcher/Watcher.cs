using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Drafts.Patterns;

namespace Drafts {

	/// <summary>Fires an event when changed.</summary>
	[Serializable]
	public class Watcher<T> {
		public Watcher() { }
		public Watcher(T value) => _value = value;

		[SerializeField] T _value;

		/// <summary>OnChanged is only called if new value is different.</summary>
		public bool ignoreEqualValues;

		/// <summary>newValue</summary>
		public UnityEvent<T> OnChanged = new UnityEvent<T>();

		/// <summary>thisWatcher, newValue, oldValue</summary>
		public UnityEvent<Watcher<T>, T, T> OnChangedAlt = new UnityEvent<Watcher<T>, T, T>();

		/// <summary>Triggers OnChanged.</summary>
		public T Value {
			get => _value;
			set {
				if(ignoreEqualValues && EqualityComparer<T>.Default.Equals(value, _value)) return;
				var old = _value;
				_value = value;
				OnChanged.Invoke(value);
				OnChangedAlt.Invoke(this, value, old);
			}
		}

		/// <summary>Triggers OnChanged.</summary>
		public void Set(T value) => Value = value;
		/// <summary>Does not trigger callbacks.</summary>
		public void SilentlySet(T value) => _value = value;

		public override string ToString() => _value?.ToString();
		public static implicit operator T(Watcher<T> w) => w._value;
	}
}

