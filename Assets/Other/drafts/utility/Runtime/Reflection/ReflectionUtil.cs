using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Drafts {
	public static partial class ReflectionUtil {

		static IEnumerable<Assembly> customAssemblies;

		/// <summary>Excludes Assemblies that starts with 'Unity' and 'System'.</summary>
		public static IEnumerable<Assembly> CustomAssemblies => customAssemblies ??=
			AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("Unity") && !a.FullName.StartsWith("System"));

		/// <summary>BindingFlags: Public, NonPublic, Instance, Static, FlattenHierarchy.</summary>
		public const BindingFlags CommonFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

		/// <summary>BindingFlags: NonPublic, Instance, Static.</summary>
		public const BindingFlags PrivateFlags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

		/// <summary>Find NonPublic Instance methods. Stop looking when base class is U.</summary>
		public static IEnumerable<FieldInfo> FindInheritedPrivateFields<U>(this Type type) {
			IEnumerable<FieldInfo> fields = new FieldInfo[0];
			while(type != null && type != typeof(U)) {
				fields = fields.Concat(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
				type = type.BaseType;
			}
			return fields;
		}
		/// <summary>Find NonPublic Instance methods. Stop looking when base class is U.</summary>
		public static IEnumerable<FieldInfo> FindSerializableFields<U>(this Type type) {
			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			bool Elegible(FieldInfo f) => f.IsPublic || f.HasAttribute(typeof(SerializeField));

			IEnumerable<FieldInfo> fields = new FieldInfo[0];

			while(type != null && type != typeof(U)) {
				fields = fields.Concat(type.GetFields(flags).Where(f => Elegible(f)));
				type = type.BaseType;
			}
			return fields;
		}

		public static IEnumerable<FieldInfo> GetInheritedFields(this Type t, BindingFlags flags = CommonFlags) {
			IEnumerable<FieldInfo> ret = t.GetFields(flags);
			while((t = t.BaseType) != null) ret.Concat(t.GetFields(flags));
			return ret;
		}

		public static IEnumerable<MemberInfo> GetInheritedMembers(this Type t, BindingFlags flags = CommonFlags) {
			IEnumerable<MemberInfo> ret = t.GetMembers(flags);
			while((t = t.BaseType) != null) ret.Concat(t.GetMembers(flags));
			return ret;
		}

		public static MemberInfo GetInheritedMember(this Type t, string name, BindingFlags flags = CommonFlags) {
			MemberInfo ret = t.GetMember(name, flags).FirstOrDefault();
			while(ret == null && (t = t.BaseType) != null) ret = t.GetMember(name, flags).FirstOrDefault();
			return ret;
		}

		public static MethodInfo GetInheritedMethod(this Type t, string name, BindingFlags flags = CommonFlags) {
			MethodInfo ret = t.GetMethods(flags).First(m => m.Name == name);
			while(ret == null && (t = t.BaseType) != null) ret = t.GetMethod(name, flags);
			return ret;
		}

		/// <summary>Get the value of a field, property or execute a method by name using reflection.</summary>
		public static void SetValue(this MemberInfo memberInfo, object instance, params object[] values) {
			switch(memberInfo.MemberType) {
				case MemberTypes.Field: ((FieldInfo)memberInfo).SetValue(instance, values[0]); break;
				case MemberTypes.Property: ((PropertyInfo)memberInfo).SetValue(instance, values[0]); break;
				case MemberTypes.Method: ((MethodInfo)memberInfo).Invoke(instance, values); break;
				default: Debug.LogError("Member is neither field, property or method."); break;
			}
		}
		/// <summary>Get the value of a field, property or parameterless function by name using reflection.</summary>
		public static object GetValue(this MemberInfo memberInfo, object instance, params object[] args) {
			switch(memberInfo.MemberType) {
				case MemberTypes.Field: return ((FieldInfo)memberInfo).GetValue(instance);
				case MemberTypes.Property: return ((PropertyInfo)memberInfo).GetValue(instance);
				case MemberTypes.Method: return ((MethodInfo)memberInfo).Invoke(instance, args);
				default: Debug.LogError("Member is neither field, property or method."); return null;
			}
		}
		/// <summary>Get the value of a field, property or parameterless function by name using reflection.</summary>
		public static T GetValue<T>(this MemberInfo memberInfo, object instance, params object[] args)
			=> (T)GetValue(memberInfo, instance, args);

		public static Type ReturnType(this MemberInfo memberInfo) {
			switch(memberInfo.MemberType) {
				case MemberTypes.Field: return ((FieldInfo)memberInfo).FieldType;
				case MemberTypes.Property: return ((PropertyInfo)memberInfo).PropertyType;
				case MemberTypes.Method: return ((MethodInfo)memberInfo).ReturnType;
				default: throw new Exception("Member is neither field, property or method.");
			}
		}

		/// <summary>Get the value of a field, property or parameterless method by name using reflection.</summary>
		public static T Reflect<T>(this object obj, string fieldName, Type type = null, BindingFlags flags = CommonFlags)
			=> Reflect<T>(obj, fieldName, null, type, flags);

		/// <summary>Get the value of a field, property or parameterless method by name using reflection.</summary>
		public static T Reflect<T>(this object obj, string fieldName, object args, Type type = null, BindingFlags flags = CommonFlags) {
			type ??= obj.GetType();
			try {
				return type.GetInheritedMember(fieldName, flags).GetValue<T>(obj, args);
			} catch {
				Debug.LogError($"Member {fieldName} not found or type not supported in {type}.");
				return default;
			}
		}

		/// <summary>Set the value of a field or property or execute a method by name using reflection.</summary>
		public static void ReflectSet(this object obj, string name, object value, BindingFlags flags = CommonFlags) {
			var member = obj.GetType().GetInheritedMember(name, flags);
			if(member == null) throw new ArgumentException($"Member {name} not found in {obj.GetType()}.");
			member.SetValue(obj, value); // or type not supported
		}

	}
}
