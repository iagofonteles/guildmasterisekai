using System;
using System.Collections.Generic;
using System.Linq;
using URand = UnityEngine.Random;

namespace Drafts {

	public static partial class ListUtil {

		/// <summary>Reorganize elements in a random order.</summary>
		public static void Shuffle<T>(this T[] l) { for(var n = 0; n < l.Length; n++) l.Swap(n, URand.Range(0, l.Length)); }
		/// <summary>Reorganize elements in a random order.</summary>
		public static void Shuffle<T>(this List<T> l) { for(var n = 0; n < l.Count; n++) l.Swap(n, URand.Range(0, l.Count)); }
		/// <summary>Add _values to the list then reorganize elements in a random order.</summary>
		public static void Shuffle<T>(this List<T> l, IEnumerable<T> values) { l.AddRange(values); l.Shuffle(); }

		/// <summary>Get a random value from the collection</summary>
		public static T Random<T>(this IReadOnlyList<T> v) => v[URand.Range(0, v.Count)];
		/// <summary>Get a random value from the collection</summary>
		public static T Random<T>(this T[] v) => v[URand.Range(0, v.Length)];
		/// <summary>Get a random value from the collection</summary>
		public static T RandomOrDefault<T>(this IReadOnlyList<T> array) => array.Count == 0 ? default : array[URand.Range(0, array.Count)];
		/// <summary>Get a random value from the collection</summary>
		public static T RandomOrDefault<T>(this T[] array) => array.Length == 0 ? default : array[URand.Range(0, array.Length)];
		/// <summary>Get a random value from the collection</summary>
		public static T Random<T>(this IEnumerable<T> v) => v.Skip(URand.Range(0, v.Count())).FirstOrDefault();

		/// <summary>Get random _values from the array. They are still ordered though.</summary>
		public static IReadOnlyList<T> Random<T>(this List<T> list, int num) {
			num = Math.Min(num, list.Count);
			var ret = new List<T>();
			for(var i = 0; i < list.Count; i++)
				if(URand.value <= (num - ret.Count) / (float)(list.Count - i))
					ret.Add(list[i]);
			return ret;
		}
		/// <summary>Get random _values from the array. They are still ordered though.</summary>
		public static T[] Random<T>(this T[] array, int num) {
			if(num < 0) throw new ArgumentException("num cannot be negative");
			num = Math.Min(num, array.Length);
			var ret = new T[num];
			var curr = 0;
			for(var i = 0; i < array.Length; i++)
				if(URand.value <= (num - curr) / (float)(array.Length - i)) { ret[curr] = array[i]; curr++; }
			return ret;
		}
		/// <summary>Get random _values from the array. They are still ordered though.</summary>
		public static IEnumerable<T> Random<T>(this IEnumerable<T> collection, int num) {
			if(num < 0) throw new ArgumentException("num cannot be negative");
			num = Math.Min(num, collection.Count());
			var ret = new T[num];
			var curr = 0;
			var length = collection.Count();
			for(var i = 0; i < length; i++)
				if(URand.value <= (num - curr) / (float)(length - i))
					ret[curr++] = collection.ElementAt(i);
			return ret;
		}

		/// <summary>Get a random element. Greater weigths are more likely to be selected.</summary>
		public static T WeightedRandom<T>(this IEnumerable<T> itens, Func<T, int> getWeight) {
			var value = URand.Range(0, itens.Sum(getWeight));
			var current = 0;
			return itens.First(i => value < (current += getWeight(i)));
		}

	}
}
