using System.Collections.Generic;
using System;

namespace Drafts {
	/// <summary>The Compare method ignore the second argument and compare with the current value.</summary>
	public class Comparer<T, P> : IComparer<T> where P : IComparable<P> {
		public static int Instance;
		Func<T, P> getProp;
		P value;
		public Comparer(Func<T, P> getProp) => this.getProp = getProp;
		public Comparer<T, P> Value(P value) { this.value = value; return this; }
		int IComparer<T>.Compare(T x, T y) => getProp(x).CompareTo(value);
	}

	public static class ExtensionsComparer {
		public static int BinarySearch<T, P>(this List<T> list, P value, Comparer<T, P> comparer)
			where P : IComparable<P> => list.BinarySearch(default, comparer.Value(value));

		public static void BinaryInsert<T>(this List<T> list, T value, Comparer<T, T> comparer)
			where T : IComparable<T> {
			var index = BinarySearch(list, value, comparer);
			if(index < 0) index = ~index;
			list.Insert(index, value);
		}
	}
}