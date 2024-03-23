using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using URand = UnityEngine.Random;

namespace Drafts {
	public static partial class ListUtil {

		/// <summary>Remove the first value and return it.</summary>
		public static T Dequeue<T>(this List<T> list) => list.Pop(0);
		/// <summary>Insert value at the end.</summary>
		public static void Enqueue<T>(this List<T> list, T value) => list.Add(value);

		/// <summary>Add the value at the end.</summary>
		public static void Push<T>(this List<T> list, T value) => list.Add(value);
		/// <summary>Add the _values at the end.</summary>
		public static void Push<T>(this List<T> list, IEnumerable<T> values) => list.AddRange(values);

		/// <summary>Remove the value at the end and return it.</summary>
		public static T Pop<T>(this List<T> list) => list.Pop(list.Count - 1);
		/// <summary>Remove the value at the end and return it.</summary>
		public static T PopOrDefault<T>(List<T> list) => list.Count > 0 ? list.Pop() : default;
		/// <summary>Remove the value at index from the list and return it.</summary>
		public static T Pop<T>(this List<T> list, int index) { var v = list[index]; list.RemoveAt(index); return v; }
		/// <summary>Remove a random value from the list and return it.</summary>
		public static T PopRandom<T>(this List<T> list) => list.Pop(URand.Range(0, list.Count));
		/// <summary>Remove [amount] _values at the end and return them.</summary>
		public static List<T> PopMany<T>(this List<T> list, int amount) => list.PopMany(list.Count - amount, amount);
		/// <summary>Remove [amount] _values at the end and return them.</summary>
		public static List<T> PopMany<T>(this List<T> list, int index, int amount) {
			try {
				var ret = list.GetRange(index, amount);
				list.RemoveRange(index, amount);
				return ret;
			} catch {
				Debug.LogError($"i:{index}, a:{amount}");
				return new List<T>();
			}
		}
		/// <summary>Remove the value at the end and return it. Retun false if list is empty.</summary>
		public static bool TryPop<T>(List<T> list, out T item) {
			if(list.Count > 0) {
				item = list.Pop();
				return true;
			}
			item = default;
			return false;
		}
		/// <summary>Remove the value that matches the predicate and return it. Return default if not found.</summary>
		public static T PopOrDefault<T>(this List<T> list, Func<T, bool> predicate) {
			var value = list.FirstOrDefault(predicate);
			list.Remove(value);
			return value;
		}
		/// <summary>Remove the value that matches the predicate and return it. Throw exception if not found.</summary>
		public static T Pop<T>(this List<T> list, Func<T, bool> predicate) {
			var value = list.First(predicate);
			list.Remove(value);
			return value;
		}
		/// <summary>Remove the value that matches the predicate and return it. Throw exception if not found.</summary>
		public static List<T> PopAll<T>(this List<T> list, Func<T, bool> predicate) {
			var ret = new List<T>(list.Where(predicate));
			list.RemoveAll(e => predicate(e));
			return ret;
		}

		/// <summary>Get the last [amount] _values on the list.</summary>
		public static List<T> Peek<T>(this List<T> list, int amount) => list.GetRange(list.Count - amount, amount);
		/// <summary>Get the last value on the list.</summary>
		public static T Peek<T>(this List<T> list) => list[list.Count - 1];

		/// <summary>Swap _values of index n with m.</summary>
		public static void Swap<T>(this List<T> l, int n, int m) { var v = l[n]; l[n] = l[m]; l[m] = v; }
		/// <summary>Swap _values of index n with m.</summary>
		public static void Swap<T>(this T[] l, int n, int m) { var v = l[n]; l[n] = l[m]; l[m] = v; }

		public static IEnumerable<T> Take<T>(this IEnumerable<T> ie, int num, Func<T, bool> filter) => ie.Where(filter).Take(num);

		/// <summary>Return Default value if key does not exists.</summary>
		[Obsolete]
		public static V GetOrDefault<K, V>(this Dictionary<K, V> c, K key) => c.TryGetValue(key, out var v) ? v : default;
		/// <summary>Return defaultValue if key does not exists.</summary>
		public static V Get<K, V>(this Dictionary<K, V> c, K key, V defaultValue) => c.TryGetValue(key, out var v) ? v : defaultValue;

		/// <summary>Initialize with Default value if key does not exists.</summary>
		public static void Init<K, V>(this Dictionary<K, V> c, K key, V value = default) { if(!c.ContainsKey(key)) c[key] = value; }

		/// <summary>Show _values on the console.</summary>
		public static string LogString<T, U>(this Dictionary<T, U> v) => v.Aggregate("", (s, p) => s += $"{p.Key}: {p.Value}\n");
		/// <summary>Show _values on the console.</summary>
		public static void Log<T, U>(this Dictionary<T, U> v) => Debug.Log(LogString(v));

		/// <summary>Show values on the console.</summary>
		public static string LogString<T>(this IEnumerable<T> v, string separator = "\n") {
			var i = 0;
			return v.Aggregate("", (s, a) => s += $"{i++}: {a}{separator}");
		}

		/// <summary>Show values on the console.</summary>
		public static string LogString(this IEnumerable v, string separator = "\n") {
			var i = 0;
			var ret = new StringBuilder();
			foreach(var item in v) ret.Append($"{i++}: {item}{separator}");
			return ret.ToString();
		}
		/// <summary>Show _values on the console.</summary>
		public static string LogString<T>(this IEnumerable<T> v, Func<T, string> select, string separator = "\n") {
			var i = 0;
			var ret = new StringBuilder();
			foreach(var item in v) ret.Append($"{i++}: {select(item)}{separator}");
			return ret.ToString();
		}

		/// <summary>Show _values on the console.</summary>
		public static void Log(this IEnumerable v) => Debug.Log(LogString(v));
		/// <summary>Show _values on the console.</summary>
		public static void Log<T>(this IEnumerable<T> v) => Debug.Log(LogString(v));
		/// <summary>Show _values on the console.</summary>
		public static void Log<T>(this IEnumerable<T> v, Func<T, string> select) => Debug.Log(LogString(v));

		/// <summary>Uses object.Equals</summary>
		public static bool AddUnique<T>(this List<T> list, T item) => list.AddUnique(item, i => i.Equals(item));
		/// <summary>Only add if no item matches.</summary>
		public static bool AddUnique<T>(this List<T> list, T item, Predicate<T> match) {
			if(list.Any(i => match(i))) return false;
			list.Add(item);
			return true;
		}

		public static void AddUnique<T>(this List<T> list, List<T> other, Predicate<T> match)
			=> other.ForEach(o => list.AddUnique(o, match));

		public static HashSet<T> Duplicates<T>(List<T> list, Func<T, T, bool> comparator) {
			var ret = new HashSet<T>();
			for(int i = 0; i < list.Count - 1; i++)
				for(int j = i + 1; j < list.Count; j++)
					if(comparator(list[i], list[j])) ret.Add(list[j]);
			return ret;
		}

		public static bool HasDuplicates<T>(List<T> list, Func<T, T, bool> comparator) {
			for(int i = 0; i < list.Count - 1; i++)
				for(int j = i + 1; j < list.Count; j++)
					if(comparator(list[i], list[j])) return true;
			return false;
		}

		public static void RemoveDuplicates<T>(this List<T> list) => list.RemoveAll(a => list.Count(b => b.Equals(a)) > 1);

		/// <summary>Returns a List with the indexes of entries that match the condition.</summary>
		public static List<int> IndexesOf<T>(this IEnumerable<T> array, Predicate<T> predicate) {
			var r = new List<int>(); var i = 0;
			foreach(var a in array) { if(predicate(a)) r.Add(i); i++; }
			return r;
		}

		public static int IndexOf<T>(this IEnumerable<T> array, T element) {
			var i = 0;
			foreach(var a in array) {
				if(element?.Equals(a) ?? a == null) return i;
				i++;
			}
			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> array, Func<T, bool> predicate) {
			var i = 0;
			foreach(var a in array) {
				if(predicate(a)) return i;
				i++;
			}
			return -1;
		}
	}
}
