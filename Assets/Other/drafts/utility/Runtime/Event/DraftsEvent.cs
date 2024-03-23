using System;
using System.Collections.Generic;
using UnityEngine;
namespace Drafts {

	[Serializable]
	public class DraftsEvent {
		[SerializeField] List<DraftsAction> list = new();
		public void Invoke() { foreach(var item in list) { item.Invoke(); } onTrigger?.Invoke(); }
		public event Action onTrigger;
		public void AddListener(Action a) => onTrigger += a;
		public void RemoveListener(Action a) => onTrigger -= a;
	}

	[Serializable]
	public class DraftsEvent<A> : List<DraftsAction<A>> {
		public void Invoke(A a) { foreach(var item in this) { item.Invoke(a); } onTrigger?.Invoke(a); }
		public event Action<A> onTrigger;
		public void AddListener(Action<A> a) => onTrigger += a;
		public void RemoveListener(Action<A> a) => onTrigger -= a;
	}

	[Serializable]
	public class DraftsEvent<A, B> : List<DraftsAction<A, B>> {
		public void Invoke(A a, B b) { foreach(var item in this) { item.Invoke(a, b); onTrigger?.Invoke(a, b); } }
		public event Action<A, B> onTrigger;
		public void AddListener(Action<A, B> a) => onTrigger += a;
		public void RemoveListener(Action<A, B> a) => onTrigger -= a;
	}

	[Serializable]
	public class DraftsEvent<A, B, C> : List<DraftsAction<A, B, C>> {
		public void Invoke(A a, B b, C c) { foreach(var item in this) { item.Invoke(a, b, c); onTrigger?.Invoke(a, b, c); } }
		public event Action<A, B, C> onTrigger;
		public void AddListener(Action<A, B, C> a) => onTrigger += a;
		public void RemoveListener(Action<A, B, C> a) => onTrigger -= a;
	}

	[Serializable]
	public class DraftsEvent<A, B, C, D> : List<DraftsAction<A, B, C, D>> {
		public void Invoke(A a, B b, C c, D d) { foreach(var item in this) { item.Invoke(a, b, c, d); onTrigger?.Invoke(a, b, c, d); } }
		public event Action<A, B, C, D> onTrigger;
		public void AddListener(Action<A, B, C, D> a) => onTrigger += a;
		public void RemoveListener(Action<A, B, C, D> a) => onTrigger -= a;
	}

}