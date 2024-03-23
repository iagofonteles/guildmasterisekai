using System;
using UnityEngine;

namespace Drafts.Patterns {

	[Serializable]
	public class SimpleStat<T> : ISimpleStat, IStatWatcher<T> {
		public object Owner { get; }
		[SerializeField] T value, @base;
		public event Action OnModified;
		public event ChangedEventHandler<T> OnStatChanged;

		public SimpleStat(T value = default) => this.value = value;
		public SimpleStat(object owner, T value = default) : this(value) => Owner = owner;

		public virtual T Base => @base;
		public virtual T Value {
			get => value;
			set {
				var old = this.value;
				if(value?.Equals(this.value) ?? this.value == null) return;
				this.value = value;
				OnModified?.Invoke();
				OnStatChanged?.Invoke(this, new(null, value, old));
			}
		}

		object ISimpleStat.Current => Value;
		object ISimpleStat.Base => Base;
		float ISimpleStat.Percent => value is int ia && @base is int ib ? ia / (float)ib
			: value is float fa && @base is float fb ? fa / fb : 1;

		public void Reset() => Value = @base;
		public override string ToString() => value.ToString();
	}
}