using System.Collections.Generic;
using System.Linq;
using System;
namespace Drafts {

	public abstract class DataView<DATA, T> : DataView<DATA> where T : DataView<DATA, T> {
		static readonly List<T> instances = new List<T>();
		public static IReadOnlyList<T> Instances => instances;

		protected virtual void Awake() => instances.Add((T)this);
		protected override void OnDestroy() { instances.Remove((T)this); base.OnDestroy(); }

		public static IEnumerable<T> Active => instances.Where(i => i.isActiveAndEnabled);
		public static IEnumerable<T> All(DATA data) => instances.Where(i => i.Data?.Equals(data) ?? data == null);
		public static IEnumerable<T> All(Func<T, bool> predicate) => instances.Where(predicate);
		public static T First(DATA data) => instances.FirstOrDefault(i => i.Data?.Equals(data) ?? data == null);
		public static T First(Func<T, bool> predicate) => instances.FirstOrDefault(predicate);

		public static implicit operator DATA(DataView<DATA, T> listener) => listener.Data;
	}

	/// <summary>Use this to sync a UI element with some data structure using events from the data structure.
	/// Can also be used to avoid destroiyng UI elements and instead.
	/// Auto Subscribe Repaint to IOnChanged on Awake.</summary>
	/// <typeparam name="T">The data structure to sync with.</typeparam>
	public abstract class DataView<T> : DataView, IDataView<T> {

		new public virtual T Data { get => (T)data; set => SetData(value); }

		protected override void SetData(object value) {

			if(data != null) Unsubscribe();

			data = value == null ? default : value is T v ? v
				: value is IData d && d.Data is T vv ? vv
				: throw new DataTypeException<T>(value, this);

			if(data != null) Subscribe();

			OnDataChanged?.Invoke(Data);
			Refresh();
		}
	}
}
