using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Drafts.EventExtensions;
using UObj = UnityEngine.Object;
namespace Drafts {

	[Serializable]
	public class DraftsAction {
		[SerializeField] protected string signature = "null";
		[SerializeField] protected UObj obj;
		[SerializeField] protected string method;
		[SerializeField] DraftsArg[] args;
		[SerializeField] bool inline;

#if UNITY_EDITOR
		MethodInfo Method => GetMethod();
		object[] Args => GetArgs();
#else
		MethodInfo _method;
		object[] _args;
		MethodInfo Method => _method ??= GetMethod();
		object[] Args => _args ??= GetArgs();
#endif
		public bool IsValid => obj;
		public object Invoke(params object[] args) => Method.Invoke(obj, inline ? Args : args);
		MethodInfo GetMethod() {
			var type = obj.GetType();
			var m = type.GetMethods().FirstOrDefault(method.MatchMethod);
			return m ?? throw new Exception($"Action parse failed: {type.Name}.{method}");
		}

		object[] GetArgs() => args.GetValues();
	}
}
