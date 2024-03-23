using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Specialized;
namespace Drafts.Patterns {

	[Serializable]
	public class ListWatcher<T> : INotifyCollectionChanged, IEnumerable<T> {
		public delegate void OnChangedHandler(object sender, object source, NotifyCollectionChangedEventArgs args);

		[SerializeField] List<T> list;
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event OnChangedHandler OnChanged;
		public int Count => list.Count;
		public T this[int index] => list[index];

		public ListWatcher() => list = new();
		public ListWatcher(IEnumerable<T> content) => list = new(content);

		public List<T> GetRange(int index, int count) => list.GetRange(index, count);

		public void Add(object source, T item) => AddRange(source, new T[] { item });
		public void Insert(object source, int index, T item) => InsertRange(source, index, new T[] { item });
		public void AddRange(object source, IEnumerable<T> items) => InsertRange(source, list.Count, items);
		public void InsertRange(object source, int index, IEnumerable<T> items) {
			list.InsertRange(index, items);
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), index);
			OnChanged?.Invoke(this, source, args);
			CollectionChanged?.Invoke(this, args);
		}

		public bool Remove(object source, T item) {
			var index = list.IndexOf(item);
			if(index < 0) return false;
			RemoveAt(source, index, 1);
			return true;
		}

		public List<T> RemoveAll(object source, Func<T, bool> predicate) {
			var result = new List<T>();
			for(int i = 0; i < list.Count;) {
				if(!predicate(list[i])) i++;
				else result.Add(RemoveAt(source, i));
			}
			return result;
		}

		public bool Replace(object source, T old, T @new) {
			var index = IndexOf(old);
			if(index < 0) return false;
			Replace(source, index, @new);
			return true;
		}

		public T Replace(object source, int index, T item) {
			throw new NotImplementedException();
			//TODO
			//var result = list[index];
			//list[index] = item;
			//var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,  );

			//OnChanged.Invoke(args);
			//CollectionChanged.Invoke(args);
			//return result;
		}

		public T RemoveAt(object source, int index) => RemoveAt(source, index, 1).FirstOrDefault();

		public IEnumerable<T> RemoveAt(object source, int index, int count = 1) {
			var items = list.GetRange(index, count);
			list.RemoveRange(index, count);
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);
			OnChanged?.Invoke(this, source, args);
			CollectionChanged?.Invoke(this, args);
			return items;
		}

		public IEnumerable<T> Clear(object source) => RemoveAt(source, 0, list.Count);

		public T Pop(object source) => RemoveAt(source, list.Count - 1);
		public T Dequeue(object source) => RemoveAt(source, 0);
		public IEnumerable<T> Pop(object source, int count = 1) => RemoveAt(source, list.Count - count, count);
		public IEnumerable<T> Dequeue(object source, int count = 1) => RemoveAt(source, 0, count);
		
		public int IndexOf(T item) => list.IndexOf(item);

		public void SilentlyAddRange(IEnumerable<T> items) => list.AddRange(items);
		public void SilentlyRemoveRange(IEnumerable<T> items) => list.AddRange(items);

		public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
	}
}