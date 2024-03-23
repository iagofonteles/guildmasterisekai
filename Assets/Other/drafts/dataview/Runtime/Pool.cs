//using System.Collections.Generic;

//namespace Drafts {

//	public abstract class IDataPool<T, D> where T : IData<D> {
//		protected Stack<T> pool = new();

//		protected abstract T Create();

//		public T Recycle(D data) {
//			var obj = pool.Count > 0 ? pool.Pop() : Create();
//			obj.Data = data;
//			return obj;
//		}

//		public void Trash(T obj) {
//			obj.Data = default;
//			pool.Push(obj);
//		}
//	}

//	public class DataViewPool<T, D> : IDataPool<T, D> where T : DataView<D> {
//		T prefab;
//		public DataViewPool(T prefab) => this.prefab = prefab;
//		protected override T Create() => UnityEngine.Object.Instantiate(prefab, prefab.transform.parent);
//	}
//}
