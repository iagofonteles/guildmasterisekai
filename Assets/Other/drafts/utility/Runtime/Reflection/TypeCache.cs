using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Drafts {

	public class HideFromTypeCache : Attribute { }

	public class TypeNotFoundExeption : Exception {
		public TypeNotFoundExeption(Type type, string name) : base($"TypeCache<{type?.Name}>: {name} not found.") { }
	}

	static class TypeCache {

		public static List<string> ExcludedAssemblies = new() {
			"unity", "cinemach", "system", "mscor",
			"ReportGen",
			"nunit", "log4net",
			"fmod", "dotween",
		};

		static IReadOnlyList<Type> all;
		public static IReadOnlyList<Type> All => all ??= GetAll();
		static Dictionary<Type, CachedDerivedTypes> fetched = new();

		class Comparer : IEqualityComparer<Type> {
			bool IEqualityComparer<Type>.Equals(Type x, Type y) => x.FullName.Equals(y.FullName);
			int IEqualityComparer<Type>.GetHashCode(Type obj) => obj.FullName.GetHashCode();
		}

		static IReadOnlyList<Type> GetAll() {
			IEnumerable<Type> all = new List<Type>();
			var comparer = new Comparer();
			var pattern = "^(" + string.Join('|', ExcludedAssemblies) + ")";
			var regex = new Regex(pattern, RegexOptions.IgnoreCase);
			var customAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !regex.IsMatch(a.FullName));

			foreach(var assembly in customAssemblies) {
				try {
					var exported = assembly.GetTypes();
					all = all.Concat(exported);
					if(exported.Count() > 200) Debug.Log(exported.Count() + ": " + assembly.GetName().Name);
				} catch(ReflectionTypeLoadException e) {
					all = all.Concat(e.Types.Where(t => t != null));
				}
			}

			all = all.Where(t => ReflectionUtil.IsConcrete(t) && t.GetCustomAttribute<HideFromTypeCache>() == null);
			return all.OrderBy(t => t.Name).ToList();
			//return all.Distinct(comparer).OrderBy(t => t.Name).ToList();
		}

		public static CachedDerivedTypes GetCache(Type type) {
			if(fetched.TryGetValue(type, out var cache)) return cache;
			return fetched[type] = new CachedDerivedTypes(type);
		}
	}

	/// <summary>
	/// exclude assemblies begining with:
	/// unity|system|mscor|ReportGen|cinemach|nunit|log4net
	/// </summary>
	public class CachedDerivedTypes {
		Type type;
		public IReadOnlyList<Type> All { get; }
		public string[] Names { get; }

		internal CachedDerivedTypes(Type t) {
			type = t;
			var all = new List<Type>();
			foreach(var type in TypeCache.All)
				try { if(t.IsAssignableFrom(type)) all.Add(type); } catch { }

			//all.Sort((a, b) => a.Name.CompareTo(b.Name));
			All = all;
			Names = All.Select(t => t.Name).ToArray();
			//All.Log();
		}

		public Type Get(string name) {
			var result = All.BinaryFindOrDefault(name, (t, s) => t.Name.CompareTo(s));
			if(result == null) throw new TypeNotFoundExeption(type, name);
			return result;
		}
	}

	/// <summary>
	/// exclude assemblies begining with:
	/// unity|system|mscor|ReportGen|cinemach|nunit|log4net
	/// </summary>
	public static class TypeCache<T> {
		static CachedDerivedTypes cache;
		public static IReadOnlyList<Type> All => cache.All;
		public static IEnumerable<string> Names => cache.Names;
		static TypeCache() => cache = TypeCache.GetCache(typeof(T));
		public static Type Get(string name) => cache.Get(name);
		public static T New(string name, params object[] args) => (T)Activator.CreateInstance(cache.Get(name), args);
		public static T[] InstantiateAll(params object[] args) => All.Select(t => (T)Activator.CreateInstance(t, args)).ToArray();
	}
}