using System;
using System.Reflection;

namespace Drafts {
	public static partial class ReflectionUtil {

		public static bool HasAttribute(this MemberInfo memberInfo, Type type) => memberInfo.GetCustomAttribute(type) != null;

		public static bool TryGetAttribute<T>(this MemberInfo memberInfo, out T attribute) where T : Attribute {
			attribute = memberInfo.GetCustomAttribute<T>();
			return attribute != null;
		}

		public static bool HasInterface(this Type t, Type i) => t.GetInterface(i.Name) != null;

		public static bool TryGetInterface<T>(this object obj, out T result) {
			var i = obj.GetType().GetInterface(typeof(T).Name);
			result = (T)i.GetValue(obj);
			return result != null;
		}

		public static bool ChildOf(this Type t, Type i) => i.IsAssignableFrom(t);
		public static bool Is(this FieldInfo f, Type t) => t.IsAssignableFrom(f.FieldType);
		public static bool Is(this PropertyInfo p, Type t) => t.IsAssignableFrom(p.PropertyType);
		public static bool IsConcrete(this Type t) => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition;
	}
}
