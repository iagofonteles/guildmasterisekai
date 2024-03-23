using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Drafts {
	public static partial class ReflectionUtil {

		/// <summary>Ignore abstract and generic types</summary>
		public static IEnumerable<Type> FindDerivedTypes(this Assembly assembly, Type baseType)
			=> FindDerivedTypes(baseType, new[] { assembly });

		/// <summary>Ignore abstract and generic types</summary>
		public static IEnumerable<Type> FindDerivedTypes(Type baseType, IEnumerable<Assembly> assemblies = null)
			=> (assemblies ?? CustomAssemblies).SelectMany(a => a.GetTypes().Where(t => t.ChildOf(baseType)));

		public static IEnumerable<(Type type, T attr)> FindTypesWith<T>(IEnumerable<Assembly> assemblies = null) where T : Attribute
			=> (assemblies ?? CustomAssemblies).SelectMany(a => a.GetTypes()
			.Select(t => (type: t, attr: t.TryGetAttribute<T>(out var a) ? a : null))
			.Where(t => t.attr != null));
	}
}
