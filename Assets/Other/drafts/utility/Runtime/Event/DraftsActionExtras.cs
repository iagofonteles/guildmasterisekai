using System.Linq;
using System.Reflection;
using UObj = UnityEngine.Object;

namespace Drafts.EventExtensions {
	public static class DraftsActionExtras {

		public static string GetNameWithParams(this MethodInfo m)
			=> $"{m.Name} ({m.GetParamsNames()})";

		public static string GetParamsNames(this MethodInfo m)
			=> string.Join(",", m.GetParameters().Select(p => p.ParameterType.Name));

		public static bool ValidParameters(this MethodInfo m) {
			if(m == null) return true;
			if(m.IsGenericMethod || m.IsConstructor) return false;

			foreach(var p in m.GetParameters()) {
				if(typeof(UObj).IsAssignableFrom(p.ParameterType)) continue;
				if(DraftsArg.compatibleTypes.ContainsKey(p.ParameterType.Name)) continue;
				return false;
			}
			return true;
		}

		public static bool MatchMethod(this string name, MethodInfo m) {
			var split = name.Split(' ');
			if(m.Name != split[0]) return false;
			var param = split[1][1..^1] + ",";
			return MatchParameters(param, m);
		}

		public static bool MatchParameters(this string signature, MethodInfo m) {
			var p = m.GetParameters();
			var split = signature.Split(',');
			if(p.Length == 0 || signature == ",") return true;
			if(p.Length != split.Length - 1) return false;

			for(int i = 0; i < split.Length - 1; i++)
				if(split[i] != p[i].ParameterType.Name) return false;
			return true;
		}

		public static bool FlexibleSignature(this string signature, MethodInfo m) {
			var split = signature.Split(',');
			if(!string.IsNullOrEmpty(split[^1]) && split[^1] != m.ReturnType.Name) return false;

			var p = m.GetParameters();
			if(split.Length > 1 && p.Length != split.Length - 1) return false;
			for(int i = 0; i < split.Length - 1; i++)
				if(split[i] != p[i].ParameterType.Name) return false;
			return true;
		}
	}
}