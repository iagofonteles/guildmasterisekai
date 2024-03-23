using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;

namespace Drafts.Patterns {

	[Serializable]
	public class SimpleListWatcher<T> : IEnumerable<T>, IReadOnlyList<T>, INotifyCollectionChanged {

		[SerializeField] List<T> list = new();
		public event Action<T> OnAdded, OnRemoved;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Count => list.Count;
		public T this[int index] => list[index];

		public void Add(T item) => Insert(list.Count, item);
		public void Insert(int index, T item) {
			list.Insert(index, item);
			OnAdded?.Invoke(item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public bool Remove(T item) {
			var index = list.IndexOf(item);
			if(index < 0) return false;
			RemoveAt(index);
			return true;
		}

		public T RemoveAt(int index) {
			var item = list[index];
			list.RemoveAt(index);
			OnRemoved?.Invoke(item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			return item;
		}

		public void Clear() {
			while(list.Count > 0) RemoveAt(0);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public T Pop() => RemoveAt(list.Count - 1);
		public T Dequeue() => RemoveAt(0);
		public int IndexOf(T item) => list.IndexOf(item);
		public int FindIndex(Predicate<T> predicate) => list.FindIndex(predicate);

		public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
	}
}