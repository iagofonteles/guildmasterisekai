using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Specialized;

namespace Drafts.Patterns {

	[Serializable]
	public class SimpleArrayWatcher<ITEM> : IEnumerable<ITEM>, IReadOnlyList<ITEM>, INotifyCollectionChanged {

		[SerializeField] ITEM[] array;
		public event Action<int, ITEM> OnChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Length => array.Length;
		int IReadOnlyCollection<ITEM>.Count => array.Length;

		public ITEM this[int index] {
			get => array[index];
			set => Set(index, value);
		}

		public SimpleArrayWatcher(int size) => array = new ITEM[size];
		public SimpleArrayWatcher(IEnumerable<ITEM> content) => array = content.ToArray();

		public void Set(int index, ITEM item) {
			var old = array[index];
			array[index] = item;

			OnChanged?.Invoke(index, item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Replace, item, old, index));
		}

		public void Clear(ITEM item) { for(int i = 0; i < array.Length; i++) Set(i, item); }
		public int IndexOf(ITEM item) => Array.IndexOf(array, item);
		public int FindIndex(Predicate<ITEM> predicate) => Array.FindIndex(array, predicate);
		public void SilentlySet(int index, ITEM item) => array[index] = item;

		public IEnumerator<ITEM> GetEnumerator() => ((IEnumerable<ITEM>)array).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();
	}
}