using System.Collections.Generic;

namespace Drafts {

	/// <summary>Create a pool of objects T to reuse, avoiding overhead of instantiation.</summary>
	public abstract class ObjectPool<T> {
		protected List<T> pool = new List<T>(); // object pool

		/// <summary>Get from the pool or call Create, then call OnRecycle.</summary>
		public T Recycle() {

			T obj;
			if(pool.Count == 0)
				obj = Create();
			else {
				obj = pool[0];
				pool.RemoveAt(0);
			}

			OnRecycle(obj);
			return obj;
		}

		/// <summary>Execute deactivation action then return the obj to the pool.</summary>
		public void Trash(T obj) {
			OnTrash(obj);
			pool.Add(obj);
		}

		/// <summary>Instantiate a batch of objects to add to the pool. Use in loading times.</summary>
		public void FillPool(int number) { for(int i = 0; i < number; i++) pool.Add(Create()); }

		/// <summary>Clear pool itens.</summary>
		public void Clear() => pool.Clear();

		public void Remove(T obj) => pool.Remove(obj);

		/// <summary>Create a new instance.</summary>
		protected abstract T Create();
		protected virtual void OnRecycle(T obj) { }
		protected virtual void OnTrash(T obj) { }
	}

	/// <summary>Create a pool of objects T to reuse, avoiding overhead of instantiation.</summary>
	public abstract class ObjectPool<T, D> {
		protected List<T> pool = new List<T>(); // object pool

		/// <summary>Get from the pool or create new if the pool is empty, then do activation action.</summary>
		public T Recycle(D data) {

			T obj;
			if(pool.Count == 0)
				obj = Create();
			else {
				obj = pool[0];
				pool.RemoveAt(0);
			}

			OnRecycle(obj, data);
			return obj;
		}

		/// <summary>Execute deactivation action then return the obj to the pool.</summary>
		public void Trash(T obj) {
			OnTrash(obj);
			pool.Add(obj);
		}

		/// <summary>Instantiate a batch of objects to add to the pool. Use in loading times.</summary>
		public void FillPool(int number) { for(int i = 0; i < number; i++) pool.Add(Create()); }

		/// <summary>Clear pool itens.</summary>
		public void Clear() => pool.Clear();

		public void Remove(T obj) => pool.Remove(obj);

		/// <summary>Create a new instance.</summary>
		protected abstract T Create();
		protected abstract void OnRecycle(T obj, D data);
		protected virtual void OnTrash(T obj) { }
	}

}