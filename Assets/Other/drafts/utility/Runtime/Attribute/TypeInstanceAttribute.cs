using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Drafts {

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public class TypeCompatibility : Attribute {
		public Type[] Types { get; }
		public TypeCompatibility(params Type[] types) => Types = types;
	}

	public class TypeInstanceAttribute : PropertyAttribute { }

	public interface ITypeCompatibility {
		Type Compatibility { get; }
	}

	[Serializable]
	public class TypeInstance<T> : IEnumerable<T> {
		[SerializeReference] public List<T> list = new();
		public void Add(T item) => list.Add(item);
		public void Remove(T item) => list.Remove(item);
		public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class TypeSearchSettings : ISearchSettings<Type> {
		CachedDerivedTypes cache;
		Func<Type, bool> filter;

		public string Title { get; }
		public IEnumerable<Type> GetItens() => cache.All.Where(filter);
		public string GetName(Type type) => type.Name;

		public TypeSearchSettings(Type type, Func<Type, bool> filter = null) {
			Title = "Subtypes: " + type.Name;
			cache = TypeCache.GetCache(type);
			this.filter = filter ?? GetCompatibilityFilter(type);
		}

		public static Func<Type, bool> GetCompatibilityFilter(Type type) {
			if(type == null) return null;
			return other => IsCompatibile(type, other);
		}

		public static bool IsCompatibile(Type a, Type b) {
			if(a == null) throw new ArgumentNullException(nameof(a));
			if(b == null) throw new ArgumentNullException(nameof(b));

			var compatibility = b.GetCustomAttribute<TypeCompatibility>();
			if(compatibility == null) return true;
			return compatibility.Types.Contains(a);
		}
	}
}
