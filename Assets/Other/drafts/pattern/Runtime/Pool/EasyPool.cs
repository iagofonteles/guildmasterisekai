using System;
using System.Collections.Generic;

namespace Drafts {

	/// <summary>Create a pool of objects T to reuse, avoiding overhead of instantiation.</summary>
	public class EasyPool<T> {
		protected List<T> pool = new List<T>(); // object pool
		public Func<T> Create;
		public Action<T> OnRecycle;
		public Action<T> OnTrash;

		public EasyPool(Func<T> create, Action<T> recycle = null, int prepool = 0) : this(create, recycle, null, prepool) { }
		public EasyPool(Func<T> create, Action<T> recycle = null, Action<T> trash = null, int prepool = 0) {
			Create = create;
			OnRecycle = recycle;
			OnTrash = trash;
			for(int i = 0; i < prepool; i++) pool.Add(Create());
		}

		/// <summary>Get from the pool or call Create, then call OnRecycle.</summary>
		public T Recycle() {

			T obj;
			if(pool.Count == 0)
				obj = Create();
			else {
				obj = pool[0];
				pool.RemoveAt(0);
			}

			OnRecycle?.Invoke(obj);
			return obj;
		}

		/// <summary>Execute deactivation action then return the obj to the pool.</summary>
		public void Trash(T obj) {
			OnTrash(obj);
			pool.Add(obj);
		}

		/// <summary>Instantiate a batch of objects to add to the pool. Use in loading times.</summary>
		public void FillPool(int number) { for(int i = 0; i < number; i++) pool.Add(Create()); }
		public void Clear() => pool.Clear();
		public void Remove(T obj) => pool.Remove(obj);
	}

	/// <summary>Create a pool of objects T to reuse, avoiding overhead of instantiation.</summary>
	public class EasyPool<T, DATA> {
		protected List<T> pool = new List<T>(); // object pool
		public Func<T> Create;
		public Action<T, DATA> OnRecycle;
		public Action<T> OnTrash;

		public EasyPool(Func<T> create, Action<T, DATA> recycle = null, int prepool = 0) : this(create, recycle, null, prepool) { }
		public EasyPool(Func<T> create, Action<T, DATA> recycle = null, Action<T> trash = null, int prepool = 0) {
			Create = create;
			OnRecycle = recycle;
			OnTrash = trash;
			for(int i = 0; i < prepool; i++) pool.Add(Create());
		}

		/// <summary>Get from the pool or call Create, then call OnRecycle.</summary>
		public T Recycle(DATA data) {

			T obj;
			if(pool.Count == 0)
				obj = Create();
			else {
				obj = pool[0];
				pool.RemoveAt(0);
			}

			OnRecycle?.Invoke(obj, data);
			return obj;
		}

		/// <summary>Execute deactivation action then return the obj to the pool.</summary>
		public void Trash(T obj) {
			OnTrash(obj);
			pool.Add(obj);
		}

		/// <summary>Instantiate a batch of objects to add to the pool. Use in loading times.</summary>
		public void FillPool(int number) { for(int i = 0; i < number; i++) pool.Add(Create()); }
		public void Clear() => pool.Clear();
		public void Remove(T obj) => pool.Remove(obj);
	}

}