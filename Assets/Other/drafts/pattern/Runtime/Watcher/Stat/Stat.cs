using System;
namespace Drafts.Patterns {

	public abstract class Stat<T> : IStat, IStatWatcher<T> {

		public object Owner { get; }
		public ValueWatcher<T> Current { get; } = new();
		public ValueWatcher<T> Base { get; } = new();
		public ValueWatcher<T> Add { get; } = new();
		public ValueWatcher<T> Mult { get; } = new();
		public ValueWatcher<T> Min { get; } = new();
		public ValueWatcher<T> Max { get; } = new();
		public T Total { get; private set; } = default;

		public event Action OnModified;
		public event Action<T> OnChanged;
		public event ChangedEventHandler<T> OnStatChanged;

		public object lastSource = null;

		object ISimpleStat.Current => Current.Value;
		object ISimpleStat.Base => Base.Value;
		object IStat.Add => Add.Value;
		object IStat.Mult => Mult.Value;
		object IStat.Min => Min.Value;
		object IStat.Max => Max.Value;
		object IStat.Total => Total;

		float ISimpleStat.Percent => Total is int ia && Base.Value is int ib ? ia / (float)ib
			: Total is float fa && Base.Value is float fb ? fa / fb : 1;

		public Stat(object owner = null) {
			Owner = owner;
			Current.OnChanged += Evaluate;
			Base.OnChanged += Evaluate;
			Add.OnChanged += Evaluate;
			Mult.OnChanged += Evaluate;
			Min.OnChanged += Evaluate;
			Max.OnChanged += Evaluate;
		}

		void Evaluate(T _) {
			var old = Total;
			Total = GetTotal();
			if(!Total.Equals(old))
				OnStatChanged?.Invoke(this, new(lastSource, Total, old));
			OnChanged?.Invoke(Total);
			OnModified?.Invoke();
		}

		public override string ToString() => Current.ToString();
		protected abstract T GetTotal();
	}
}
