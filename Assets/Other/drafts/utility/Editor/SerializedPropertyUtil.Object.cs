using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
public static partial class SerializedPropertyUtil {

	static object GetRelativeField(object obj, IEnumerable<string> path) {
		foreach(var n in path) {
			if(obj == null) break;

			if(n.Contains('[')) {
				var name = n.Substring(0, n.IndexOf('['));
				var index = n.Substring(name.Length + 1, n.Length - name.Length - 2);
				var arrayField = GetFieldInBase(obj.GetType(), name);
				var array = arrayField.GetValue(obj);
				var indexer = array.GetType().GetProperty("Item");
				obj = indexer.GetValue(array, new object[] { int.Parse(index) });
				continue;
			}
			var field = GetFieldInBase(obj.GetType(), n);
			obj = field.GetValue(obj);
		}
		return obj;
	}

	public static object GetObject(this SerializedProperty property) {
		object obj = property.serializedObject.targetObject;
		var path = property.propertyPath.Replace(".Array.data", "").Split('.');
		return GetRelativeField(obj, path);
	}

	public static object GetParentObject(this SerializedProperty property) {
		object obj = property.serializedObject.targetObject;
		var path = property.propertyPath.Replace(".Array.data", "").Split('.');
		return GetRelativeField(obj, path.SkipLast(1));
	}

	public static FieldInfo GetFieldInBase(Type type, string fieldName, BindingFlags flags = Drafts.ReflectionUtil.CommonFlags) {
		do {
			var f = type.GetField(fieldName, flags);
			if(f != null) return f;
			type = type.BaseType;
		} while(type != null);
		throw new Exception($"Field {fieldName} not found");
	}
}
