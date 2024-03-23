using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
namespace Drafts.Patterns {

	public class DictionaryWatcher<KEY, VALUE> : INotifyCollectionChanged, IEnumerable<KeyValuePair<KEY, VALUE>> {
		Dictionary<KEY, VALUE> map = new();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public VALUE this[KEY key] => map[key];

		public void Add(KEY key, VALUE value) {
			if(map.ContainsKey(key)) throw new System.ArgumentException($"Duplicated key {key}.");
			Replace(key, value);
		}

		public void Replace(KEY key, VALUE value) {
			NotifyCollectionChangedEventArgs args;
			if(map.ContainsKey(key)) {
				args = new(NotifyCollectionChangedAction.Replace,
					new KeyValuePair<KEY, VALUE>[] { new(key, value) });
			} else {
				args = new(NotifyCollectionChangedAction.Add,
					new KeyValuePair<KEY, VALUE>[] { new(key, value) });
			}
			map[key] = value;
			CollectionChanged?.Invoke(this, args);
		}

		public bool Remove(KEY key) => Remove(key, out _);

		public bool Remove(KEY key, out VALUE value) {
			if(!map.Remove(key, out value)) return false;
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
				new KeyValuePair<KEY, VALUE>[] { new(key, value) });
			CollectionChanged?.Invoke(this, args);
			return true;
		}

		public void Clear() {
			map.Clear();
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			CollectionChanged?.Invoke(this, args);
		}

		public bool TryGetValue(KEY key, out VALUE value) {
			value = default;
			return key != null && map.TryGetValue(key, out value);
		}

		public VALUE GetValueOrDefault(KEY key) => TryGetValue(key, out var value) ? value : default;

		public IEnumerator<KeyValuePair<KEY, VALUE>> GetEnumerator() => map.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => map.GetEnumerator();
	}

}
