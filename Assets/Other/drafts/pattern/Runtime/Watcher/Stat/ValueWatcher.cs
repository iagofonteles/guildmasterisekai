using System;
using UnityEngine;
namespace Drafts.Patterns {
	[Serializable]
	public class ValueWatcher<T> {
		[SerializeField] T value;
		public event Action<T> OnChanged;
		public ValueWatcher(T value = default) => this.value = value;

		public virtual T Value {
			get => value;
			set {
				if(value?.Equals(this.value) ?? this.value == null) return;
				this.value = value;
				OnChanged?.Invoke(value);
			}
		}
		public override string ToString() => value.ToString();
	}
}