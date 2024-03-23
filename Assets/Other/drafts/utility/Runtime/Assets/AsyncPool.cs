//using System.Collections.Generic;
//using System;
//using UnityEngine;
//using System.Collections;

//namespace Drafts {
//	public class AsyncPool<T> where T : Component {

//		readonly Stack<T> pool = new();
//		readonly Async<T> prefab;
//		readonly Action<T> OnRecycle;
//		readonly Action<T> OnTrash;
//		public IEnumerator Wait => prefab.Wait;

//		public AsyncPool(string path, Action<T> onRecycle = null, Action<T> onTrash = null) {
//			prefab = path;
//			OnRecycle = onRecycle ?? (c => c.gameObject.SetActive(true));
//			OnTrash = onTrash ?? (c => c.gameObject.SetActive(false));
//		}

//		public T Recycle() {
//			var ret = pool.Count > 0 ? pool.Pop() : UnityEngine.Object.Instantiate(prefab.Value);
//			OnRecycle(ret); return ret;
//		}

//		public void RecycleAsync(Action<T> callback) => prefab.UseAsync(c => callback(Recycle()));
//		public void Trash(T item) { OnTrash(item); pool.Push(item); }
//	}

//	public class AsyncPool<T, D> : AsyncPool<T> where T : Component {
//		readonly Action<T, D> SetData;

//		public AsyncPool(string path, Action<T, D> setData = null, Action<T> onRecycle = null, Action<T> onTrash = null)
//			: base(path, onRecycle, onTrash) { SetData = setData; }

//		public T Recycle(D data) { var ret = Recycle(); SetData(ret, data); return ret; }
//		public void RecycleAsync(D data, Action<T> callback) => RecycleAsync(c => { SetData(c, data); callback(c); });
//	}

//#if DRAFTS_DATAVIEW
//	public class AsyncIDataPool<T, D> : AsyncPool<T, D> where T : Component, IData<D> {
//		public AsyncIDataPool(string path, Action<T> onRecycle = null, Action<T> onTrash = null)
//			: base(path, (c, d) => c.Data = d, onRecycle, onTrash) { }
//	}
//#endif

//}