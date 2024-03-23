using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Specialized;
using UnityEditor;

namespace Drafts.Patterns {

	[Serializable]
	public class ArrayWatcher<ITEM> : ArrayWatcher<ITEM, object, ArrayWatcher<ITEM>> {
		public ArrayWatcher(int size) : base(size) { }
		public ArrayWatcher(IEnumerable<ITEM> content) : base(content) { }
	}

	[Serializable]
	public class ArrayWatcher<ITEM, SOURCE> : ArrayWatcher<ITEM, SOURCE, ArrayWatcher<ITEM, SOURCE>> {
		public ArrayWatcher(int size) : base(size) { }
		public ArrayWatcher(IEnumerable<ITEM> content) : base(content) { }
	}

	[Serializable]
	public class ArrayWatcher<ITEM, SOURCE, T> : IEnumerable<ITEM>, INotifyCollectionChanged
		where T : ArrayWatcher<ITEM, SOURCE, T> {
		public delegate void Callback(T array, SOURCE source, int index, ITEM value);

		[SerializeField] ITEM[] array;
		public event Callback OnChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Length => array.Length;
		public ITEM this[int index] => array[index];

		public ArrayWatcher(int size) => array = new ITEM[size];
		public ArrayWatcher(IEnumerable<ITEM> content) => array = content.ToArray();

		public void Set(SOURCE source, int index, ITEM item) {
			var old = array[index];
			array[index] = item;
			OnChanged?.Invoke((T)this, source, index, item);
			var args = new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Replace, item, old, index);
			CollectionChanged?.Invoke(this, args);
		}

		public void Clear(SOURCE source, ITEM item) {
			for(int i = 0; i < array.Length; i++) Set(source, i, item);
			//var args = new NotifyCollectionChangedEventArgs(
			//	NotifyCollectionChangedAction.Replace, array, 0);
			//CollectionChanged?.Invoke(this, args);
		}

		public bool Add(SOURCE source, ITEM item) {
			var index = Array.IndexOf(array, default);
			if(index < 0) return false;
			Set(source, index, item);
			return true;
		}

		public bool Remove(SOURCE source, ITEM item) {
			var index = Array.IndexOf(array, item);
			if(index < 0) return false;
			Set(source, index, default);
			return true;
		}

		public void SilentlySet(int index, ITEM item) {
			var old = array[index];
			array[index] = item;
			var args = new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Replace, item, old, index);
			CollectionChanged?.Invoke(this, args);
		}

		public IEnumerator<ITEM> GetEnumerator() => ((IEnumerable<ITEM>)array).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();
	}
}