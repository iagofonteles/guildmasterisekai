using System;
using System.Collections.Generic;

namespace Drafts {
	public static partial class SearchUtil {

		public class EntryNotFoundException : Exception { 
			public EntryNotFoundException(object item) :base($"{item} is not in the collection or the collection is not sorted.") { }
		}

		public static L BinaryFind<L, T>(this L[] list, T item, Func<L, T, int> comparator) {
			var id = list.BinarySearch(item, comparator);
			return id < 0 ? throw new EntryNotFoundException(item) : list[id];
		}

		public static L BinaryFind<L, T>(this L[] list, T item) where L : IComparable<T> {
			var id = list.BinarySearch(item);
			return id < 0 ? throw new EntryNotFoundException(item) : list[id];
		}

		public static L BinaryFind<L, T>(this IReadOnlyList<L> list, T item, Func<L, T, int> comparator) {
			var id = list.BinarySearch(item, comparator);
			return id < 0 ? throw new EntryNotFoundException(item) : list[id];
		}

		public static L BinaryFind<L, T>(this IReadOnlyList<L> list, T item) where L : IComparable<T> {
			var id = list.BinarySearch(item);
			return id < 0 ? throw new EntryNotFoundException(item) : list[id];
		}

		public static L BinaryFindOrDefault<L, T>(this L[] list, T item) where L : IComparable<T> {
			var id = list.BinarySearch(item);
			return id < 0 ? default : list[id];
		}

		public static L BinaryFindOrDefault<L, T>(this L[] list, T item, Func<L, T, int> comparator) {
			var id = list.BinarySearch(item, comparator);
			return id < 0 ? default : list[id];
		}

		public static L BinaryFindOrDefault<L, T>(this IReadOnlyList<L> list, T item) where L : IComparable<T> {
			var id = list.BinarySearch(item);
			return id < 0 ? default : list[id];
		}

		public static L BinaryFindOrDefault<L, T>(this IReadOnlyList<L> list, T item, Func<L, T, int> comparator) {
			var id = list.BinarySearch(item, comparator);
			return id < 0 ? default : list[id];
		}

	}
}