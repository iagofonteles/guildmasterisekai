using System;
using System.Collections.Generic;
using UnityEngine;
using UObj = UnityEngine.Object;
namespace Drafts {

	[Serializable]
	public class DraftsArg {

		public static Dictionary<string, Func<DraftsArg, object>> compatibleTypes = new() {
			{ typeof(string).Name, a => a.value },
			{ typeof(float).Name, a => (float)a },
			{ typeof(int).Name, a => (int)a },
			{ typeof(bool).Name, a => (bool)a},
		};

		[SerializeField] string type;
		[SerializeField] UObj objValue;
		[SerializeField] string value;

		public static implicit operator UObj(DraftsArg args) => args.objValue;
		public static implicit operator string(DraftsArg args) => args.value;
		public static implicit operator int(DraftsArg args) => int.Parse(args.value);
		public static implicit operator float(DraftsArg args) => float.Parse(args.value);
		public static implicit operator bool(DraftsArg args) => bool.Parse(args.value);

		public T GetValue<T>() where T : UObj => (T)objValue;
		public object GetValue() => compatibleTypes.TryGetValue(type, out var f) ? f(this) : objValue;
	}

	public static class ExtensionsUnityArg {
		public static object[] GetValues(this DraftsArg[] args) {
			var result = new object[args.Length];
			for(int i = 0; i < args.Length; i++)
				result[i] = args[i].GetValue();
			return result;
		}
	}
}