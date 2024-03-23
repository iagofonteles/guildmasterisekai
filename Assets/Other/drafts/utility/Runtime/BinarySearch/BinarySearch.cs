using System;
using System.Collections.Generic;

namespace Drafts {
	public static partial class SearchUtil {

		public static int BinarySearch<L, T>(this L[] list, T item, Func<L, T, int> comparator) {
			int left = 0, right = list.Length - 1, id, result;

			while(left <= right) {
				id = (left + right) / 2;
				result = comparator(list[id], item);
				if(result == 0) return id;
				if(result < 0) left = id + 1;
				else right = id - 1;
			}
			return ~left;
		}

		public static int BinarySearch<L, T>(this IReadOnlyList<L> list, T item, Func<L, T, int> comparator) {
			int left = 0, right = list.Count - 1, id, result;

			while(left <= right) {
				id = (left + right) / 2;
				result = comparator(list[id], item);
				if(result == 0) return id;
				if(result < 0) left = id + 1;
				else right = id - 1;
			}
			return ~left;
		}

		public static int BinarySearch<L, T>(this L[] list, T item) where L : IComparable<T> => BinarySearch(list, item, (l, t) => l.CompareTo(t));
		public static int BinarySearch<L, T>(this IReadOnlyList<L> list, T item) where L : IComparable<T> => BinarySearch(list, item, (l, t) => l.CompareTo(t));

	}
}