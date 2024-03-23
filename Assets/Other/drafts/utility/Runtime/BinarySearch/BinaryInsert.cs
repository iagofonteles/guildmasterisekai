using System;
using System.Collections.Generic;

namespace Drafts {

	public static partial class SearchUtil {

		///// <summary>Considering the list is already sorted, add the element.</summary>
		public static void BinaryInsert<T>(this List<T> list, T element) {
			var index = list.BinarySearch(element);
			if(index < 0) index = ~index;
			list.Insert(index, element);
		}

		/// <summary>Considering the list is already sorted, add the element </summary>
		public static void BinaryInsert<T>(this List<T> list, T element, IComparer<T> comparer) {
			var index = list.BinarySearch(element, comparer);
			if(index < 0) index = ~index;
			list.Insert(index, element);
		}

		/// <summary>Considering the list is already sorted, add the element </summary>
		public static void BinaryInsert<T>(this List<T> list, T element, Func<T, T, int> comparator) {
			var index = list.BinarySearch(element, comparator);
			if(index < 0) index = ~index;
			list.Insert(index, element);
		}

		/// <summary>Considering the list is already sorted, add the element. Return false on duplicate found.</summary>
		public static bool BinaryInsertUnique<T>(this List<T> list, T element) {
			var index = list.BinarySearch(element);
			if(index < 0) index = ~index;
			else return false;
			list.Insert(index, element);
			return true;
		}

		/// <summary>Considering the list is already sorted, add the element </summary>
		public static bool BinaryInsertUnique<T>(this List<T> list, T element, IComparer<T> comparer) {
			var index = list.BinarySearch(element, comparer);
			if(index < 0) index = ~index;
			else return false;
			list.Insert(index, element);
			return true;
		}

		/// <summary>Considering the list is already sorted, add the element </summary>
		public static bool BinaryInsertUnique<T>(this List<T> list, T element, Func<T, T, int> comparator) {
			var index = list.BinarySearch(element, comparator);
			if(index < 0) index = ~index;
			else return false;
			list.Insert(index, element);
			return true;
		}

		///// <summary>Considering the list is already sorted, add the element.</summary>
		public static bool BinaryRemove<T>(this List<T> list, T element) {
			var index = list.BinarySearch(element);
			if(index < 0) return false;
			list.RemoveAt(index);
			return true;
		}

		/// <summary>Considering the list is already sorted, add the element </summary>
		public static bool BinaryRemove<T, K>(this List<T> list, K key) where T : IComparable<K> {
			var index = list.BinarySearch(key);
			if(index < 0) return false;
			list.RemoveAt(index);
			return true;
		}

		/// <summary>Considering the list is already sorted, add the element </summary>
		public static bool BinaryRemove<T, K>(this List<T> list, K key, Func<T, K, int> comparator) {
			var index = list.BinarySearch(key, comparator);
			if(index < 0) return false;
			list.RemoveAt(index);
			return true;
		}


	}
}