using System;
using UnityEngine;

namespace Drafts.Patterns {

	[Serializable]
	public class MonoPool<T> : ObjectPool<T> where T : Component {

		public T prefab;
		public Transform parent;
		public Action<T> onRecycle;
		public Action<T> onTrash;

		public MonoPool(T prefab, Transform parent = null) {
			this.prefab = prefab;
			this.parent = parent;
		}

		protected override T Create() => UnityEngine.Object.Instantiate(prefab, parent);
		protected override void OnRecycle(T obj) {
			obj.gameObject.SetActive(true);
			onRecycle(obj);

		}
		protected override void OnTrash(T obj) {
			obj.gameObject.SetActive(false);
			onTrash(obj);
		}
	}
}