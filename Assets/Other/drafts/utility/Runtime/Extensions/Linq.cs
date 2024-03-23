using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using URand = UnityEngine.Random;
namespace Drafts.Linq {
	public static class Linq {

		public static ArrayList ToList(this IEnumerable collection) {
			var result = new ArrayList();
			foreach(var item in collection) result.Add(item);
			return result;
		}

		public static bool Contains(this IEnumerable collection, object value) {
			foreach(var item in collection)
				if(item == value) return true;
			return false;
		}

		public static IEnumerable Where(this IEnumerable collection, Func<object, bool> predicate) {
			foreach(var item in collection)
				if(predicate(item)) yield return item;
		}

		public static IEnumerable<T> Select<T>(this IEnumerable collection, Func<object, T> select) {
			foreach(var item in collection) yield return select(item);
		}

		public static IEnumerable<T> SelectMany<T>(this IEnumerable collection, Func<object, IEnumerable<T>> select) {
			foreach(var item in collection) {
				var items = select(item);
				foreach(var subItem in items)
					yield return subItem;
			}
		}

		public static IEnumerable SelectMany(this IEnumerable collection, Func<object, IEnumerable> select) {
			foreach(var item in collection) {
				var items = select(item);
				foreach(var subItem in items)
					yield return subItem;
			}
		}

		public static IEnumerable<T> Select<I, T>(this IEnumerable collection, Func<I, T> select) {
			foreach(I item in collection) yield return select(item);
		}

		public static bool Any(this IEnumerable collection) => collection.GetEnumerator().MoveNext();

		public static bool Any(this IEnumerable collection, Func<object, bool> predicate) {
			foreach(var item in collection)
				if(predicate(item)) return true;
			return false;
		}

		public static bool All(this IEnumerable collection, Func<object, bool> predicate) {
			foreach(var item in collection)
				if(!predicate(item)) return false;
			return true;
		}

		public static IEnumerable Append(this IEnumerable collection, object value) {
			foreach(var item in collection) yield return item;
			yield return value;
		}

		public static IEnumerable Concat(this IEnumerable collection, IEnumerable other) {
			foreach(var item in collection) yield return item;
			foreach(var item in other) yield return item;
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, params IEnumerable<T>[] others) {
			foreach(var item in collection) yield return item;
			foreach(var col in others) foreach(var item in col) yield return item;
		}

		public static int Count(this IEnumerable collection) {
			var result = 0;
			foreach(var item in collection) result++;
			return result;
		}

		public static object First(this IEnumerable collection) {
			foreach(var item in collection) return item;
			return null;
		}

		public static T First<T>(this IEnumerable collection) {
			foreach(var item in collection)
				if(item is T v) return v;
			return default;
		}

		public static T First<T>(this IEnumerable collection, Func<T, bool> predicate) {
			foreach(var item in collection)
				if(item is T v && predicate(v)) return v;
			return default;
		}

		public static IEnumerable Skip(this IEnumerable collection, int count) {
			foreach(var item in collection)
				if(count-- <= 0) yield return item;
		}

		public static IEnumerable Take(this IEnumerable collection, int count) {
			foreach(var item in collection) {
				if(count-- <= 0) break;
				yield return item;
			}
		}

		public static object ElementAt(this IEnumerable collection, int index) {
			return collection.Skip(index).First();
		}

		public static object Random(this IEnumerable collection) => Random(collection, 1).First();

		/// <summary>Get random _values from the collection. They are still ordered though.</summary>
		public static IEnumerable Random(this IEnumerable collection, int num) {
			var array = collection.Cast<object>().ToArray();
			num = Math.Min(num, array.Length);
			var ret = new object[num];
			var curr = 0;
			var length = array.Length;
			for(var i = 0; i < length; i++)
				if(URand.value <= (num - curr) / (float)(length - i))
					ret[curr++] = array[i];
			return ret;
		}

		public static bool SameElements(this IEnumerable a, IEnumerable b) => a.All(b.Contains) && b.All(a.Contains);

		public static IEnumerable<T> CastOrDefault<T>(this IEnumerable collection) where T : class {
			foreach(var item in collection) yield return item as T;
		}

		public static IEnumerable<T> Cast<T>(this IEnumerable collection) {
			foreach(var item in collection) yield return (T)item;
		}

		public static IEnumerable ToIE(this object value) {
			return Enumerable.Repeat(value, 1);
		}

		public static IEnumerable<T> ToIE<T>(this T value) {
			return Enumerable.Repeat(value, 1);
		}
	}
}